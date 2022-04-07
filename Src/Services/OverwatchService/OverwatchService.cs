using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using Hangfire;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OWArcadeBackend.Dtos.Overwatch;
using OWArcadeBackend.Models.Constants;
using OWArcadeBackend.Services.TwitterService;
using OWArcadeBackend.Validators;

namespace OWArcadeBackend.Services.OverwatchService
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
                response.Data = await _unitOfWork.DailyRepository.GetDaily(overwatchType);
                response.Data.Contributor.Profile = null;
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
            DailyValidator validator = new DailyValidator(_unitOfWork, overwatchType);
            ValidationResult result = await validator.ValidateAsync(daily);

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

        /// <summary>
        /// If the ConnectToTwitter boolean is set to true
        /// execute the PostTweet method in the TwitterService
        /// </summary>
        /// <param name="overwatchType"></param>
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
                IEnumerable<Daily> dailyOwModes =
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

        public async Task<ServiceResponse<DailyDto>> GetDaily()
        {
            ServiceResponse<DailyDto> serviceResponse = new ServiceResponse<DailyDto>
            {
                Data = await _unitOfWork.DailyRepository.GetDaily(Game.OVERWATCH)
            };

            serviceResponse.Data.Contributor.Profile = null;
            
            return serviceResponse;
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
            return new ServiceResponse<List<Label>>
            {
                Data = _unitOfWork.OverwatchRepository.GetLabels()
            };
        }
    }
}