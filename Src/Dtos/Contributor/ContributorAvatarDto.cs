using Microsoft.AspNetCore.Http;

namespace OWArcadeBackend.Dtos.Contributor
{
    public class ContributorAvatarDto
    {
        public IFormFile Avatar { get; set; }
    }
}