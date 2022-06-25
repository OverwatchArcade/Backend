using AutoMapper;
using FluentValidation;
using ImageMagick;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Factories.Interfaces;
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
        private readonly ILogger<ContributorService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IValidator<ContributorAvatarDto> _contributeAvatarValidator;
        private readonly IValidator<ContributorProfileDto> _contributorProfileValidator;
        private readonly IServiceResponseFactory<ContributorDto> _serviceResponseFactory;

        public ContributorService(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ContributorService> logger, IWebHostEnvironment webHostEnvironment, IValidator<ContributorAvatarDto> contributeAvatarValidator,
            IValidator<ContributorProfileDto> contributorProfileValidator, IServiceResponseFactory<ContributorDto> serviceResponseFactory)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _contributeAvatarValidator = contributeAvatarValidator ?? throw new ArgumentNullException(nameof(contributeAvatarValidator));
            _contributorProfileValidator = contributorProfileValidator ?? throw new ArgumentNullException(nameof(contributorProfileValidator));
            _serviceResponseFactory = serviceResponseFactory ?? throw new ArgumentNullException(nameof(serviceResponseFactory));
        }

        public async Task<ServiceResponse<List<ContributorDto>>> GetAllContributors()
        {
            var serviceResponse = new ServiceResponse<List<ContributorDto>>();
            var contributors = await _unitOfWork.ContributorRepository.GetAll();
            
            contributors = contributors.OrderByDescending(c => c.Stats?.ContributionCount ?? 0).ToList();
            serviceResponse.Data = _mapper.Map<List<ContributorDto>>(contributors);

            return serviceResponse;
        }

        public ServiceResponse<ContributorDto> GetContributorByUsername(string username)
        {
            try
            {
                var contributor = _unitOfWork.ContributorRepository.Find(c => c.Username.Equals(username)).Single();
                var contributorDto = _mapper.Map<ContributorDto>(contributor);
                return _serviceResponseFactory.Create(contributorDto);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Contributor not found - " + e.Message);
                return _serviceResponseFactory.Error(404, $"Contributor {username} not found");
            }
        }
        
        public async Task<ServiceResponse<ContributorDto>> SaveProfile(ContributorProfileDto contributorProfileDto, Guid userId)
        {
            var result = await _contributorProfileValidator.ValidateAsync(contributorProfileDto);
            if (!result.IsValid)
            {
                return _serviceResponseFactory.Error(403, result.Errors.Select(err => err.ErrorMessage).ToArray());
            }

            try
            {
                var contributor = _unitOfWork.ContributorRepository.Find(c => c.Id.Equals(userId)).Single();
                contributor.Profile = _mapper.Map<ContributorProfile>(contributorProfileDto);

                await _unitOfWork.Save();
                var contributorDto = _mapper.Map<ContributorDto>(contributor);
                return _serviceResponseFactory.Create(contributorDto);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Couldn't save profile - " + e.Message);
                return _serviceResponseFactory.Error(500, "Profile couldn't be updated");
            }
        }

        public async Task<ServiceResponse<ContributorDto>> SaveAvatar(ContributorAvatarDto contributorAvatarDto, Guid userId)
        {
            var result = await _contributeAvatarValidator.ValidateAsync(contributorAvatarDto);
            if (!result.IsValid)
            {
                return _serviceResponseFactory.Error(403, result.Errors.Select(err => err.ErrorMessage).ToArray());
            }
            
            try
            {
                var contributor = _unitOfWork.ContributorRepository.Find(c => c.Id.Equals(userId)).Single();
                contributor.Avatar = await UploadAvatar(contributorAvatarDto.Avatar, contributor);
                await _unitOfWork.Save();
                var contributorDto = _mapper.Map<ContributorDto>(contributor);
                
                return _serviceResponseFactory.Create(contributorDto);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Couldn't save profile - " + e.Message);
                return _serviceResponseFactory.Error(500, "Profile couldn't be updated");
            }
        }

        private async Task<string> UploadAvatar(IFormFile file, Contributor contributor)
        {
            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.GetFullPath(_webHostEnvironment.WebRootPath + ImageConstants.ProfileFolder + fileName);
            if (!Directory.Exists(_webHostEnvironment.WebRootPath + ImageConstants.ProfileFolder))
            {
                Directory.CreateDirectory(_webHostEnvironment.WebRootPath + ImageConstants.ProfileFolder);
            }

            await using var fileStream = File.Create(filePath);
            await file.CopyToAsync(fileStream);
            await fileStream.FlushAsync();

            // Cleanup old image
            if (!contributor.HasDefaultAvatar())
            {
                var oldImage = Path.GetFullPath(_webHostEnvironment.WebRootPath + ImageConstants.ProfileFolder + contributor.Avatar);
                File.Delete(oldImage);
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
 