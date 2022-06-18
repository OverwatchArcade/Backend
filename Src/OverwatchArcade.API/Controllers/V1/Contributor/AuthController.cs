using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverwatchArcade.API.Services.AuthService;
using OverwatchArcade.API.Services.ContributorService;

namespace OverwatchArcade.API.Controllers.V1.Contributor
{
    [ApiController]
    [Route("api/v1/contributor/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IContributorService _contributorService;

        public AuthController(IAuthService authService, IContributorService contributorService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _contributorService = contributorService ?? throw new ArgumentNullException(nameof(contributorService));
        }
        
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string code, string redirectUri)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(redirectUri))
            {
                return BadRequest();
            }
            
            var response = await _authService.RegisterAndLogin(code, redirectUri);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            return Ok();
        }

        [Authorize]
        [HttpGet("Info")]
        public IActionResult Info()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? throw new Exception("User not found in JWT");
            var response = _contributorService.GetContributorByUsername(username);
            return StatusCode(response.StatusCode, response);
        }
    }
}