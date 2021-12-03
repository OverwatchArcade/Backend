using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.Results;
using Hangfire;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        public OverwatchService(ILogger<OverwatchService> logger, IUnitOfWork unitOfWork, IMemoryCache memoryCache, ITwitterService twitterService, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _twitterService = twitterService ?? throw new ArgumentNullException(nameof(twitterService));
        }

        public async Task<ServiceResponse<DailyDto>> Submit(Daily daily, Game overwatchType, Guid userId)
        {
            ServiceResponse<DailyDto> response = new ServiceResponse<DailyDto>();
            DailyValidator validator = new DailyValidator(_unitOfWork, overwatchType);
            ValidationResult result = await validator.ValidateAsync(daily);
            daily.ContributorId = userId;

            if (!result.IsValid)
            {
                response.SetError(500, string.Join(", ", result.Errors));
                return response;
            }

            if (await _unitOfWork.DailyRepository.HasDailySubmittedToday(overwatchType))
            {
                response.SetError(409, "Daily has already been submitted");
                return response;
            }

            try
            {
                _unitOfWork.DailyRepository.Add(daily);
                await _unitOfWork.Save();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                response.SetError(500, e.Message);
                return response;
            }

            var isPostingToTwitter = _configuration.GetValue<bool>("connectToTwitter");
            _logger.LogInformation($"Posting to twitter is: {(isPostingToTwitter ? "Enabled" : "Disabled")}");
            
            if (isPostingToTwitter)
            {
                BackgroundJob.Enqueue(() => _twitterService.Handle(overwatchType));
            }

            response.Data = await _unitOfWork.DailyRepository.GetDaily(overwatchType);
            response.Data.Contributor.Profile = null;

            // Set cache
            var endOfDayInUtc = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59, 999);
            _memoryCache.Set(CacheKeys.OverwatchDaily, response, endOfDayInUtc);
            
            return response;
        }

        public async Task<ServiceResponse<DailyDto>> Undo(Game overwatchType, Guid userId, bool hardDelete)
        {
            ServiceResponse<DailyDto> response = new ServiceResponse<DailyDto>();
            if (!await _unitOfWork.DailyRepository.HasDailySubmittedToday(overwatchType))
            {
                response.SetError(500, "Daily has not been submitted yet");
                return response;
            }
            
            _memoryCache.Remove(CacheKeys.OverwatchDaily);

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
                return response;
            }

            return response;
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
            ServiceResponse<List<ArcadeModeDto>> serviceResponse = new ServiceResponse<List<ArcadeModeDto>>
            {
                Data = _unitOfWork.OverwatchRepository.GetArcadeModes(Game.OVERWATCH)
            };

            return serviceResponse;
        }
        
        public ServiceResponse<List<Label>> GetLabels()
        {
            ServiceResponse<List<Label>> serviceResponse = new ServiceResponse<List<Label>>
            {
                Data = _unitOfWork.OverwatchRepository.GetLabels()
            };

            return serviceResponse;
        }
    }
}