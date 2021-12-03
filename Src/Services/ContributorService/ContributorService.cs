using AutoMapper;
using Microsoft.Extensions.Logging;
using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OWArcadeBackend.Dtos;

namespace OWArcadeBackend.Services.ContributorService
{
    public class ContributorService : IContributorService
    {
        private readonly ILogger<ContributorService> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ContributorService(ILogger<ContributorService> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

        public async Task<ServiceResponse<List<ContributorDto>>> GetAllContributors()
        {
            ServiceResponse<List<ContributorDto>> serviceResponse = new ServiceResponse<List<ContributorDto>>();
            List<Contributor> contributors = await _unitOfWork.ContributorRepository.GetAll() as List<Contributor>;
            foreach (var contributor in contributors)
            {
                contributor.Stats = await GetContributorStats(contributor.Id);
            }
            contributors = contributors.OrderByDescending(c => c.Stats.ContributionCount).ToList();
            
            serviceResponse.Data = _mapper.Map<List<ContributorDto>>(contributors);
            return serviceResponse;
        }

        public async Task<ServiceResponse<ContributorDto>> GetContributorByUsername(string username)
        {
            ServiceResponse<ContributorDto> serviceResponse = new ServiceResponse<ContributorDto>();
            try
            {
                var contributor = _unitOfWork.ContributorRepository.Find(c => c.Username.Equals(username)).Single();
                contributor.Stats = await GetContributorStats(contributor.Id);
                serviceResponse.Data = _mapper.Map<ContributorDto>(contributor);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Contributor not found - " + e.Message);
                serviceResponse.StatusCode = 404;
                serviceResponse.Message = $"Contributor {username} not found";
            }

            return serviceResponse;
        }
    }
}
