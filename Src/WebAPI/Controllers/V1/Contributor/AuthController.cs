using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Application.Contributor.Queries.GetContributor;
using OverwatchArcade.Persistence.Services.Interfaces;

namespace WebAPI.Controllers.V1.Contributor
{
    [ApiController]
    [Route("api/v1/contributor/[controller]")]
    public class AuthController : ApiControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(ILoginService loginService, ICurrentUserService currentUserService)
        {
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }


        [HttpPost("Login")]
        
        public async Task<IActionResult> Login(string code, string redirectUri)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(redirectUri))
            {
                return BadRequest();
            }
            
            var response = await _loginService.Login(code, redirectUri);
            return StatusCode(response.StatusCode, response.Success ? response.Data : response.ErrorMessages);
        }
        
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            return Ok();
        }

        [Authorize]
        [HttpGet("Info")]
        public async Task<IActionResult> Info()
        {
            var contributorDto = await Mediator.Send(new GetContributorQuery()
            {
                UserId = Guid.Parse(_currentUserService.UserId!)
            });

            if (contributorDto is null)
            {
                return NotFound();
            }

            return Ok(contributorDto);
        }
    }
}