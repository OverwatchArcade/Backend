using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Services.ContributorService;

namespace OverwatchArcade.API.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ContributorController : Controller
    {
        private readonly IContributorService _contributorService;

        public ContributorController(IContributorService contributorService)
        {
            _contributorService = contributorService ?? throw new ArgumentNullException(nameof(contributorService));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContributors()
        {
            var response = await _contributorService.GetAllContributors();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{username}")]
        public IActionResult GetContributorByUsername(string username)
        {
            var response = _contributorService.GetContributorByUsername(username);
            return StatusCode(response.StatusCode, response);
        }
        
        [Authorize]
        [HttpPost("profile")]
        public async Task<IActionResult> SaveProfile(ContributorProfileDto contributorProfile)
        {
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not found in JWT"));
            var response = await _contributorService.SaveProfile(contributorProfile, userId);
            return StatusCode(response.StatusCode, response);
        }
        
        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> SaveAvatar([FromForm] ContributorAvatarDto contributorAvatarDto)
        {
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not found in JWT"));
            var response = await _contributorService.SaveAvatar(contributorAvatarDto, userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
