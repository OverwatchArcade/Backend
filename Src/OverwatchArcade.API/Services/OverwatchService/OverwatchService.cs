using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.API.Services.ConfigService;
using OverwatchArcade.API.Services.ContributorService;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence;
using OverwatchArcade.Persistence.Repositories.Interfaces;
using OverwatchArcade.Twitter.Services.TwitterService;

namespace OverwatchArcade.API.Services.OverwatchService
{
    public class OverwatchService : IOverwatchService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfigService _configService;
        private readonly IConfiguration _configuration;
        private readonly ITwitterService _twitterService;
        private readonly ILogger<OverwatchService> _logger;
        private readonly IValidator<CreateDailyDto> _validator; 
        private readonly IContributorService _contributorService;
        private readonly IContributorRepository _contributorRepository;

        public OverwatchService(IMapper mapper, IUnitOfWork unitOfWork, IMemoryCache memoryCache,
            IConfigService configService, IConfiguration configuration, ITwitterService twitterService,
            ILogger<OverwatchService> logger, IValidator<CreateDailyDto> validator,
            IContributorService contributorService, IContributorRepository contributorRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _twitterService = twitterService ?? throw new ArgumentNullException(nameof(twitterService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _contributorService = contributorService ?? throw new ArgumentNullException(nameof(contributorService));
            _contributorRepository =
                contributorRepository ?? throw new ArgumentNullException(nameof(contributorRepository));
        }
        
        public async Task<ServiceResponse<DailyDto>> Submit(CreateDailyDto createDailyDto, Guid userId)
        {
            var serviceResponse = new ServiceResponse<DailyDto>();
            // Used for race conditions, db transaction might be too slow
            if (_memoryCache.Get<bool>(CacheKeys.OverwatchDailySubmit))
            {
                _logger.LogInformation("Race Condition (cache), daily already been submitted");
                serviceResponse.SetError(409, "Daily might currently being submitted, please try again");
                return serviceResponse;
            }
            _memoryCache.Set(CacheKeys.OverwatchDailySubmit, true, DateTimeOffset.UtcNow.AddSeconds(3));
            
            var validatorResponse = await SubmitValidator(createDailyDto, serviceResponse);
            if (!validatorResponse.Success)
            {
                return serviceResponse;
            }
            
            try
            {
                var contributor = await _contributorRepository.FirstOrDefaultASync(c => c.Id.Equals(userId));
                if (contributor is null)
                {
                    serviceResponse.SetError(500, "Contributor not found");
                    return serviceResponse;
                }

                await SubmitDailyToDatabase(createDailyDto, userId, contributor!, serviceResponse);
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Error occured submitting daily - {e.Message}");
                serviceResponse.SetError(500, e.Message);
                return serviceResponse;
            }

            await CreateAndPostTweet();
            var endOfDayInUtc = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59, 999);
            _memoryCache.Set(CacheKeys.OverwatchDaily, serviceResponse, endOfDayInUtc);
            return serviceResponse;
        }

        public async Task<ServiceResponse<DailyDto>> Undo(Guid userId, bool hardDelete)
        {
            var serviceResponse = new ServiceResponse<DailyDto>();
            var contributor = await _contributorRepository.FirstOrDefaultASync(c => c.Id.Equals(userId));
            if (contributor is null)
            {
                serviceResponse.SetError(500, "Contributor not found");
                return serviceResponse;
            }
            
            if (!await _unitOfWork.DailyRepository.HasDailySubmittedToday())
            {
                _logger.LogInformation($"{userId} tried to undo a daily that hasn't been submitted yet");
                serviceResponse.SetError(500, "Daily has not been submitted yet");
                return serviceResponse;
            }
            
            var isPostingToTwitter = _configuration.GetValue<bool>("connectToTwitter");
            if (isPostingToTwitter && hardDelete)
            {
                // Delete tweet
                _logger.LogInformation("Deleting tweet");
                _ = _twitterService.DeleteLastTweet();
            }

            _memoryCache.Remove(CacheKeys.OverwatchDaily);

            if (hardDelete)
            {
                await _unitOfWork.DailyRepository.HardDeleteDaily();
            }
            else
            {
                await _unitOfWork.DailyRepository.SoftDeleteDaily();
            }
            
            return serviceResponse;
        }
        
        public ServiceResponse<DailyDto> GetDaily()
        {
            var daily = _unitOfWork.DailyRepository.GetDaily();
            var dailyDto = _mapper.Map<DailyDto>(daily);
            
            dailyDto.IsToday = daily.CreatedAt >= DateTime.UtcNow.Date && !daily.MarkedOverwrite;
            dailyDto.Contributor.RemoveDetailedInformation();

            return new ServiceResponse<DailyDto>
            {
                Data = dailyDto
            };
        }

        public ServiceResponse<List<ArcadeModeDto>> GetArcadeModes()
        {
            var arcadeModes = _unitOfWork.OverwatchRepository.GetArcadeModes();
            return new ServiceResponse<List<ArcadeModeDto>>
            {
                Data = _mapper.Map<List<ArcadeModeDto>>(arcadeModes)
            };
        }
        
        public ServiceResponse<List<Label>> GetLabels()
        {
            var labels = _unitOfWork.OverwatchRepository.GetLabels();
            return new ServiceResponse<List<Label>>
            {
                Data = labels
            };
        }
        
        private async Task SubmitDailyToDatabase(CreateDailyDto createDailyDto, Guid userId, Contributor contributor, ServiceResponse<DailyDto> response)
        {
            var daily = _mapper.Map<Daily>(createDailyDto);
            daily.ContributorId = contributor.Id;
            _unitOfWork.DailyRepository.Add(daily);

            await _unitOfWork.Save(); // Save to get recalculation of contributor stats
            contributor.Stats = await _contributorService.GetContributorStats(userId);
            await _unitOfWork.Save();

            _logger.LogInformation($"New daily submitted by {contributor.Username}");
            var dailyDto = _mapper.Map<DailyDto>(_unitOfWork.DailyRepository.GetDaily());
            dailyDto.IsToday = daily.CreatedAt >= DateTime.UtcNow.Date && !daily.MarkedOverwrite;
            dailyDto.Contributor.RemoveDetailedInformation();
            response.Data = dailyDto;
        }
        
        private async Task CreateAndPostTweet()
        {
            var isPostingToTwitter = _configuration.GetValue<bool>("connectToTwitter");
            _logger.LogDebug($"Posting to twitter is: {(isPostingToTwitter ? "Enabled" : "Disabled")}");

            if (!isPostingToTwitter) return;

            var screenshotUrl = _configuration.GetValue<string>("ScreenshotUrl");
            var currentEvent = (await _configService.GetCurrentOverwatchEvent()).Data ?? "default";

            _ = _twitterService.PostTweet(screenshotUrl, currentEvent);
        }

        private async Task<ServiceResponse<DailyDto>> SubmitValidator(CreateDailyDto createDailyDto, ServiceResponse<DailyDto> response)
        {
            var result = await _validator.ValidateAsync(createDailyDto);

            if (!result.IsValid)
            {
                response.SetError(500, string.Join(", ", result.Errors));
                return response;
            }
            
            if (await _unitOfWork.DailyRepository.HasDailySubmittedToday())
            {
                response.SetError(409, "Daily has already been submitted");
            }

            return response;
        }
    }
}