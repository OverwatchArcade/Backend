using AutoMapper;
using FluentValidation.Results;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.API.Services.TwitterService;
using OverwatchArcade.API.Validators;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence.Persistence;
using OWArcadeBackend.Dtos.Overwatch;

namespace OverwatchArcade.API.Services.OverwatchService
{
    public class OverwatchService : IOverwatchService
    {
        private readonly ILogger<OverwatchService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly ITwitterService _twitterService;
        private readonly IMapper _mapper;

        public OverwatchService(ILogger<OverwatchService> logger, IUnitOfWork unitOfWork, IMemoryCache memoryCache, ITwitterService twitterService, IConfiguration configuration, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _twitterService = twitterService ?? throw new ArgumentNullException(nameof(twitterService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ServiceResponse<DailyDto>> Submit(Daily daily, Game overwatchType, Guid userId)
        {
            var response = new ServiceResponse<DailyDto>();
            var validatorResponse = await SubmitValidator(daily, overwatchType, response);
            if (!validatorResponse.Success)
            {
                return response;
            }
            
            try
            {
                // Used for race conditions, db transaction might be too slow
                _memoryCache.Set(CacheKeys.OverwatchDailySubmit, true, DateTimeOffset.Now.AddSeconds(1));
                daily.ContributorId = userId;
                _unitOfWork.DailyRepository.Add(daily);
                await _unitOfWork.Save();

                var dailyDto = _mapper.Map<DailyDto>(_unitOfWork.DailyRepository.GetDaily(overwatchType));
                response.Data = dailyDto;
                
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                response.SetError(500, e.Message);
                return response;
            }

            CreateAndPostTweet(overwatchType);
            SetDailyCache(response);
            return response;
        }

        private async Task<ServiceResponse<DailyDto>> SubmitValidator(Daily daily, Game overwatchType, ServiceResponse<DailyDto> response)
        {
            var validator = new DailyValidator(_unitOfWork, overwatchType);
            var result = await validator.ValidateAsync(daily);

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
            else if (await _unitOfWork.DailyRepository.HasDailySubmittedToday(overwatchType))
            {
                response.SetError(409, "Daily has already been submitted");
            }

            return response;
        }
        
        private void CreateAndPostTweet(Game overwatchType)
        {
            var isPostingToTwitter = _configuration.GetValue<bool>("connectToTwitter");
            _logger.LogInformation($"Posting to twitter is: {(isPostingToTwitter ? "Enabled" : "Disabled")}");

            if (isPostingToTwitter)
            {
                BackgroundJob.Enqueue(() => _twitterService.PostTweet(overwatchType));
            }
        }

        private void SetDailyCache(ServiceResponse<DailyDto> response)
        {
            var endOfDayInUtc = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59, 999);
            _memoryCache.Set(CacheKeys.OverwatchDaily, response, endOfDayInUtc);
        }
        
        public async Task<ServiceResponse<DailyDto>> Undo(Game overwatchType, Guid userId, bool hardDelete)
        {
            ServiceResponse<DailyDto> response = new ServiceResponse<DailyDto>();
            if (!await _unitOfWork.DailyRepository.HasDailySubmittedToday(overwatchType))
            {
                response.SetError(500, "Daily has not been submitted yet");
                return response;
            }
            var isPostingToTwitter = _configuration.GetValue<bool>("connectToTwitter");
            if (isPostingToTwitter && hardDelete)
            {
                BackgroundJob.Enqueue(() => _twitterService.DeleteLastTweet());
            }

            _memoryCache.Remove(CacheKeys.OverwatchDaily);
            await UndoFromDatabase(overwatchType, userId, hardDelete, response);
            return response;
        }

        private async Task UndoFromDatabase(Game overwatchType, Guid userId, bool hardDelete, ServiceResponse<DailyDto> response)
        {
            try
            {
                var dailyOwModes =
                    _unitOfWork.DailyRepository.Find(d => d.CreatedAt >= DateTime.UtcNow.Date && d.Game == overwatchType);

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
                _logger.LogInformation($"Daily {overwatchType} undo by uid: {userId}");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                response.SetError(500, e.Message);
            }
        }

        public ServiceResponse<DailyDto> GetDaily()
        {
            var daily = _unitOfWork.DailyRepository.GetDaily(Game.OVERWATCH);
            return new ServiceResponse<DailyDto>
            {
                Data = _mapper.Map<DailyDto>(daily)
            };
        }

        public ServiceResponse<List<ArcadeModeDto>> GetArcadeModes()
        {
            var arcadeModes = _unitOfWork.OverwatchRepository.GetArcadeModes(Game.OVERWATCH);
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