using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Contributor;

namespace OverwatchArcade.API.Services.ContributorService
{
    public interface IContributorService
    {
        Task<ServiceResponse<List<ContributorDto>>> GetAllContributors();
        public ServiceResponse<ContributorDto> GetContributorByUsername(string username);
    }
}
