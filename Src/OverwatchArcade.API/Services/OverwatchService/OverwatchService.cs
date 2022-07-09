using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.API.Services.ConfigService;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence;
using OverwatchArcade.Persistence.Repositories.Interfaces;
using OverwatchArcade.Twitter.Dtos;
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
        private readonly IContributorRepository _contributorRepository;

        public OverwatchService(IMapper mapper, IUnitOfWork unitOfWork, IMemoryCache memoryCache, IConfigService configService, IConfiguration configuration, ITwitterService twitterService, ILogger<OverwatchService> logger, IValidator<CreateDailyDto> validator, IContributorRepository contributorRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _twitterService = twitterService ?? throw new ArgumentNullException(nameof(twitterService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _contributorRepository = contributorRepository ?? throw new ArgumentNullException(nameof(contributorRepository));
        }


        public async Task<ServiceResponse<DailyDto>> Submit(CreateDailyDto createDailyDto, Guid userId)
        {
            var response = new ServiceResponse<DailyDto>();
            var validatorResponse = await SubmitValidator(createDailyDto, response);
            if (!validatorResponse.Success)
            {
                return response;
            }
            
            try
            {
                // Used for race conditions, db transaction might be too slow
                _memoryCache.Set(CacheKeys.OverwatchDailySubmit, true, DateTimeOffset.Now.AddSeconds(1));
                
                var contributor = await _contributorRepository.FirstOrDefaultASync(c => c.Id.Equals(userId));
                var daily = _mapper.Map<Daily>(createDailyDto);
                daily.ContributorId = contributor.Id;
                _unitOfWork.DailyRepository.Add(daily);
                await _unitOfWork.Save(); // Save to get recalculation of contributor stats

                contributor.Stats = await GetContributorStats(userId);
                daily.ContributorId = userId;
                
                await _unitOfWork.Save();

                var dailyDto = _mapper.Map<DailyDto>(_unitOfWork.DailyRepository.GetDaily());
                dailyDto.IsToday = daily.CreatedAt >= DateTime.UtcNow.Date && !daily.MarkedOverwrite;
                response.Data = dailyDto;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                response.SetError(500, e.Message);
                return response;
            }

            var currentEvent = (await _configService.GetCurrentOverwatchEvent()).Data ?? "default";
            var screenshotUrl =  _configuration.GetValue<string>("ScreenshotUrl");

            CreateAndPostTweet(currentEvent, screenshotUrl);
            SetDailyCache(response);
            return response;
        }
        
        /// <summary>
        /// Returns contribution stats such as count, favourite day, last contributed
        /// When a <see cref="Contributor"/> has no contributions, return empty stats.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<ContributorStats> GetContributorStats(Guid userId)
        {
            var stats = new ContributorStats()
            {
                ContributionCount = await _unitOfWork.DailyRepository.GetContributedCount(userId),
            };

            if (stats.ContributionCount <= 0) return stats;
            
            stats.ContributionCount += await _unitOfWork.DailyRepository.GetLegacyContributionCount(userId);
            stats.LastContributedAt = await _unitOfWork.DailyRepository.GetLastContribution(userId);
            stats.FavouriteContributionDay = _unitOfWork.DailyRepository.GetFavouriteContributionDay(userId);
            stats.ContributionDays = _unitOfWork.DailyRepository.GetContributionDays(userId);

            return stats;
        }

        private async Task<ServiceResponse<DailyDto>> SubmitValidator(CreateDailyDto createDailyDto, ServiceResponse<DailyDto> response)
        {
            var result = await _validator.ValidateAsync(createDailyDto);

            if (!result.IsValid)
            {
                response.SetError(500, string.Join(", ", result.Errors));
                return response;
            }

            // Used for race conditions, db transaction might be too slow
            if (_memoryCache.Get<bool>(CacheKeys.OverwatchDailySubmit))
            {
                response.SetError(409, "Daily has already been submitted");
            }
            else if (await _unitOfWork.DailyRepository.HasDailySubmittedToday())
            {
                response.SetError(409, "Daily has already been submitted");
            }

            return response;
        }
        
        private async Task CreateAndPostTweet(string currentEvent, string screenshotUrl)
        {
            var isPostingToTwitter = _configuration.GetValue<bool>("connectToTwitter");
            _logger.LogInformation($"Posting to twitter is: {(isPostingToTwitter ? "Enabled" : "Disabled")}");

            if (!isPostingToTwitter) return;
            
            var tweetDto = new CreateTweetDto()
            {
                CurrentEvent = currentEvent,
                ScreenshotUrl = screenshotUrl
            };

            await _twitterService.PostTweet(tweetDto);
        }

        private void SetDailyCache(ServiceResponse<DailyDto> response)
        {
            var endOfDayInUtc = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59, 999);
            _memoryCache.Set(CacheKeys.OverwatchDaily, response, endOfDayInUtc);
        }
        
        public async Task<ServiceResponse<DailyDto>> Undo(Guid userId, bool hardDelete)
        {
            ServiceResponse<DailyDto> response = new ServiceResponse<DailyDto>();
            if (!await _unitOfWork.DailyRepository.HasDailySubmittedToday())
            {
                response.SetError(500, "Daily has not been submitted yet");
                return response;
            }
            var isPostingToTwitter = _configuration.GetValue<bool>("connectToTwitter");
            if (isPostingToTwitter && hardDelete)
            {
                // Delete tweet
                await _twitterService.DeleteLastTweet();
            }

            _memoryCache.Remove(CacheKeys.OverwatchDaily);
            await UndoFromDatabase(userId, hardDelete, response);
            return response;
        }

        private async Task UndoFromDatabase(Guid userId, bool hardDelete, ServiceResponse<DailyDto> response)
        {
            try
            {
                var dailyOwModes =
                    _unitOfWork.DailyRepository.Find(d => d.CreatedAt >= DateTime.UtcNow.Date);

                if (hardDelete)
                {
                    _unitOfWork.DailyRepository.RemoveRange(dailyOwModes);
                }
                else
                {
                    foreach (var daily in dailyOwModes)
                    {
                        daily.MarkedOverwrite = true;
                    }
                }

                await _unitOfWork.Save();
                _logger.LogInformation($"Daily undo by uid: {userId}");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                response.SetError(500, e.Message);
            }
        }

        public ServiceResponse<DailyDto> GetDaily()
        {
            var daily = _unitOfWork.DailyRepository.GetDaily();
            var dailyDto = _mapper.Map<DailyDto>(daily);
            
            dailyDto.IsToday = daily.CreatedAt >= DateTime.UtcNow.Date && !daily.MarkedOverwrite;
            if (dailyDto.Contributor.Stats != null) dailyDto.Contributor.Stats.ContributionDays = null; // Remove contribution datetimes
            dailyDto.Contributor.Profile = null; // Remove profile
            
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
    }
}