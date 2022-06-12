using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Domain.Models.ContributorProfile;

namespace OverwatchArcade.API.Services.AuthService
{
    public interface IAuthService
    {
        public Task<ServiceResponse<string>> RegisterAndLogin(string discordBearerToken, string redirectUri);
        public Task<ServiceResponse<ContributorDto>> SaveProfile(ContributorProfile data, Guid userId);
        public Task<ServiceResponse<ContributorDto>> UploadAvatar(ContributorAvatarDto data, Guid userId);
    }
}