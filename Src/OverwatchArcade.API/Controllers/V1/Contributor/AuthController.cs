using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OverwatchArcade.API.Controllers.V1.Contributor
{
    [ApiController]
    [Route("api/v1/contributor/[controller]")]
    public class AuthController : ApiControllerBase
    {

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
            var response = _contributorService.GetContributorByUsername(username);
            return StatusCode(response.StatusCode, response);
        }
    }
}