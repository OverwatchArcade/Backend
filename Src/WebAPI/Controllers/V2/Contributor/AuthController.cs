using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Application.Contributor.Queries.GetContributor;
using OverwatchArcade.Application.Contributor.Queries.GetContributors;
using OverwatchArcade.Persistence.Services.Interfaces;
using OverwatchArcade.WebAPI.Controllers.V2;

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


        [HttpPost("Login/Discord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> Login(string discordAccessToken)
        {
            if (string.IsNullOrWhiteSpace(discordAccessToken))
            {
                return BadRequest();
            }

            try
            {
                return Ok(await _loginService.Login(discordAccessToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            return Ok();
        }

        [Authorize]
        [HttpGet("Info")]
        [ProducesDefaultResponseType(typeof(ContributorDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Info()
        {
            var contributorDto = await Mediator.Send(new GetContributorDtoQuery()
            {
                UserId = _currentUserService.UserId
            });

            if (contributorDto is null)
            {
                return NotFound();
            }

            return Ok(contributorDto);
        }
    }
}