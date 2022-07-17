using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.Domain.Models.ContributorInformation;

namespace OverwatchArcade.API.Services.ContributorService
{
    public interface IContributorService
    {
        Task<ServiceResponse<List<ContributorDto>>> GetAllContributors();
        public ServiceResponse<ContributorDto> GetContributorByUsername(string username);
        public Task<ContributorStats> GetContributorStats(Guid userId);
        public Task<ServiceResponse<ContributorDto>> SaveProfile(ContributorProfileDto contributorProfileDto, Guid userId);
        public Task<ServiceResponse<ContributorDto>> SaveAvatar(ContributorAvatarDto contributorAvatarDto, Guid userId);
    }
}
