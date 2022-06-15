using AutoMapper;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Services.ContributorService
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
        
        public async Task<ServiceResponse<List<ContributorDto>>> GetAllContributors()
        {
            var serviceResponse = new ServiceResponse<List<ContributorDto>>();
            var contributors = await _unitOfWork.ContributorRepository.GetAll();
            contributors = contributors.OrderByDescending(c => c.Stats.ContributionCount).ToList();
            
            serviceResponse.Data = _mapper.Map<List<ContributorDto>>(contributors);
            return serviceResponse;
        }

        public ServiceResponse<ContributorDto> GetContributorByUsername(string username)
        {
            ServiceResponse<ContributorDto> serviceResponse = new ServiceResponse<ContributorDto>();
            try
            {
                var contributor = _unitOfWork.ContributorRepository.Find(c => c.Username.Equals(username)).Single();
                serviceResponse.Data = _mapper.Map<ContributorDto>(contributor);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Contributor not found - " + e.Message);
                serviceResponse.SetError(404, $"Contributor {username} not found");
            }

            return serviceResponse;
        }
    }
}
 