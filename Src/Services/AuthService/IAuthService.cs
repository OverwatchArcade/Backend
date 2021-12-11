using System;
using System.Threading.Tasks;
using OWArcadeBackend.Dtos.Contributor;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Services.AuthService
{
    public interface IAuthService
    {
        public Task<ServiceResponse<string>> RegisterAndLogin(string discordBearerToken, string redirectUri);
        public Task<ServiceResponse<ContributorDto>> SaveProfile(ContributorProfile data, Guid userId);
        public Task<ServiceResponse<ContributorDto>> UploadAvatar(ContributorAvatarDto data, Guid userId);
    }
}