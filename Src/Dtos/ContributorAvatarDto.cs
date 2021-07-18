using Microsoft.AspNetCore.Http;

namespace OWArcadeBackend.Dtos
{
    public class ContributorAvatarDto
    {
        public IFormFile Avatar { get; set; }
    }
}