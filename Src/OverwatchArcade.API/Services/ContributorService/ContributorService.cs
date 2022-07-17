using AutoMapper;
using FluentValidation;
using ImageMagick;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Factories.Interfaces;
using OverwatchArcade.API.Utility;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Services.ContributorService
{
    public class ContributorService : IContributorService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileProvider _fileProvider;
        private readonly ILogger<ContributorService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IValidator<ContributorAvatarDto> _contributorAvatarValidator;
        private readonly IValidator<ContributorProfileDto> _contributorProfileValidator;
        private readonly IServiceResponseFactory<ContributorDto> _serviceResponseFactory;

        public ContributorService(IMapper mapper, IUnitOfWork unitOfWork, IFileProvider fileProvider, ILogger<ContributorService> logger, IWebHostEnvironment webHostEnvironment, IValidator<ContributorAvatarDto> contributeAvatarValidator,
            IValidator<ContributorProfileDto> contributorProfileValidator, IServiceResponseFactory<ContributorDto> serviceResponseFactory)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _contributorAvatarValidator = contributeAvatarValidator ?? throw new ArgumentNullException(nameof(contributeAvatarValidator));
            _contributorProfileValidator = contributorProfileValidator ?? throw new ArgumentNullException(nameof(contributorProfileValidator));
            _serviceResponseFactory = serviceResponseFactory ?? throw new ArgumentNullException(nameof(serviceResponseFactory));
        }

        public async Task<ServiceResponse<List<ContributorDto>>> GetAllContributors()
        {
            var serviceResponse = new ServiceResponse<List<ContributorDto>>();
            var contributors = (await _unitOfWork.ContributorRepository.GetAll()).ToList();

            contributors.OrderByDescending(c => c.Stats?.ContributionCount ?? 0).ToList().ForEach(c =>
            {
                c.Profile = null;
                if (c.Stats != null) c.Stats.ContributionDays = null;
            });

            serviceResponse.Data = _mapper.Map<List<ContributorDto>>(contributors);

            return serviceResponse;
        }
        
        /// <summary>
        /// Returns contribution stats such as count, favourite day, last contributed
        /// When a <see cref="Contributor"/> has no contributions, return empty stats.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ContributorStats> GetContributorStats(Guid userId)
        {
            var stats = new ContributorStats()
            {
                ContributionCount = await _unitOfWork.ContributorRepository.GetContributedCount(userId),
            };

            if (stats.ContributionCount <= 0) return stats;
            
            stats.ContributionCount += await _unitOfWork.ContributorRepository.GetLegacyContributionCount(userId);
            stats.LastContributedAt = await _unitOfWork.ContributorRepository.GetLastContribution(userId);
            stats.FavouriteContributionDay = _unitOfWork.ContributorRepository.GetFavouriteContributionDay(userId);
            stats.ContributionDays = _unitOfWork.ContributorRepository.GetContributionDays(userId);

            return stats;
        }

        public ServiceResponse<ContributorDto> GetContributorByUsername(string username)
        {
            var contributor = _unitOfWork.ContributorRepository.FirstOrDefault(c => c.Username.Equals(username));
            if (contributor is null)
            {
                return _serviceResponseFactory.Error(404, $"Contributor {username} not found");
            }

            var contributorDto = _mapper.Map<ContributorDto>(contributor);
            return _serviceResponseFactory.Create(contributorDto);
        }

        public async Task<ServiceResponse<ContributorDto>> SaveProfile(ContributorProfileDto contributorProfileDto, Guid userId)
        {
            var result = await _contributorProfileValidator.ValidateAsync(contributorProfileDto);
            if (!result.IsValid)
            {
                return _serviceResponseFactory.Error(403, result.Errors.Select(err => err.ErrorMessage).ToArray());
            }

            var contributor = await _unitOfWork.ContributorRepository.FirstOrDefaultASync(c => c.Id.Equals(userId));
            if (contributor is null)
            {
                return _serviceResponseFactory.Error(500, $"Contributor with userid {userId} not found");
            }

            contributor.Profile = _mapper.Map<ContributorProfile>(contributorProfileDto);
            await _unitOfWork.Save();
            var contributorDto = _mapper.Map<ContributorDto>(contributor);
            return _serviceResponseFactory.Create(contributorDto);
        }

        public async Task<ServiceResponse<ContributorDto>> SaveAvatar(ContributorAvatarDto contributorAvatarDto, Guid userId)
        {
            var result = await _contributorAvatarValidator.ValidateAsync(contributorAvatarDto);
            if (!result.IsValid)
            {
                return _serviceResponseFactory.Error(403, result.Errors.Select(err => err.ErrorMessage).ToArray());
            }

            var contributor = await _unitOfWork.ContributorRepository.FirstOrDefaultASync(c => c.Id.Equals(userId));
            if (contributor is null)
            {
                return _serviceResponseFactory.Error(404, $"Contributor with userid {userId} not found");
            }

            try
            {
                contributor.Avatar = await UploadAvatar(contributorAvatarDto.Avatar, contributor);
                await _unitOfWork.Save();
            }
            catch (Exception e)
            {
                return _serviceResponseFactory.Error(500, e.Message);
            }
            
            var contributorDto = _mapper.Map<ContributorDto>(contributor);

            return _serviceResponseFactory.Create(contributorDto);
        }

        private async Task<string> UploadAvatar(IFormFile file, Contributor contributor)
        {
            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.GetFullPath(_webHostEnvironment.WebRootPath + ImageConstants.ProfileFolder + fileName);
            if (!_fileProvider.DirectoryExists(_webHostEnvironment.WebRootPath + ImageConstants.ProfileFolder))
            {
                _fileProvider.CreateDirectory(_webHostEnvironment.WebRootPath + ImageConstants.ProfileFolder);
            }

            try
            {
                await  _fileProvider.CreateFile(filePath, file);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Couldn't upload avatar: {e.Message}");
                throw;
            }

            // Cleanup old image
            if (!contributor.HasDefaultAvatar())
            {
                var oldImage = Path.GetFullPath(_webHostEnvironment.WebRootPath + ImageConstants.ProfileFolder + contributor.Avatar);
                _fileProvider.DeleteFile(oldImage);
            }

            await CompressImage(filePath);

            return fileName;
        }


        private async Task CompressImage(string filePath)
        {
            try
            {
                using var image = new MagickImage(filePath);
                var size = new MagickGeometry(250, 250)
                {
                    IgnoreAspectRatio = true
                };

                image.Resize(size);
                await image.WriteAsync(filePath);

                var optimizer = new ImageOptimizer();
                optimizer.Compress(filePath);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Compression failed but avatar uploaded - " + e.Message);
            }
        }
    }
}