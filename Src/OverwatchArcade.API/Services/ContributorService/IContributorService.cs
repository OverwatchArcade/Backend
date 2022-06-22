using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.Domain.Models.ContributorInformation;

namespace OverwatchArcade.API.Services.ContributorService
{
    public interface IContributorService
    {
        Task<ServiceResponse<List<ContributorDto>>> GetAllContributors();
        public ServiceResponse<ContributorDto> GetContributorByUsername(string username);
        public Task<ServiceResponse<ContributorDto>> SaveProfile(ContributorProfileDto contributorProfile, Guid userId);
    }
}
