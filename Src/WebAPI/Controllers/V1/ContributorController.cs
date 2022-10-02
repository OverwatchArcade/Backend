using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverwatchArcade.Application.Contributor.Commands.SaveAvatar;
using OverwatchArcade.Application.Contributor.Commands.SaveProfile;
using OverwatchArcade.Application.Contributor.Queries.GetContributor;
using OverwatchArcade.Application.Contributor.Queries.GetContributors;

namespace WebAPI.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ContributorController : ApiControllerBase
    {
        [HttpGet]
        [Produces(typeof(IEnumerable<ContributorDto>))]
        [ProducesResponseType(typeof(IEnumerable<ContributorDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllContributors(CancellationToken cancellationToken)
        {
            var contributorDtos = await Mediator.Send(new GetContributorsQuery(), cancellationToken);
            
            return Ok(contributorDtos);
        }

        [HttpGet("{username}")]
        [ProducesResponseType(typeof(ContributorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContributorByUsername(string username, CancellationToken cancellationToken)
        {
            var query = new GetContributorDtoQuery()
            {
                Username = username
            };
            
            var contributorDto = await Mediator.Send(query, cancellationToken);
            if (contributorDto is null)
            {
                return NotFound();
            }
            
            return Ok(contributorDto);
        }
        
        [Authorize]
        [HttpPost("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveProfile(SaveProfileCommand saveProfileCommand, CancellationToken cancellationToken)
        {
            try
            {
                await Mediator.Send(saveProfileCommand, cancellationToken);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        
        [Authorize]
        [RequestSizeLimit(750000)]
        [HttpPost("avatar")]
        public async Task<IActionResult> SaveAvatar([FromForm] IFormFile avatar, CancellationToken cancellationToken)
        {
            var imageData = new byte[avatar.Length];
            await using (var stream = avatar.OpenReadStream())
            {
                await stream.ReadAsync(imageData, cancellationToken);
            }
            var saveAvatarCommand = new SaveAvatarCommand(imageData, avatar.ContentType);

            try
            {
                await Mediator.Send(saveAvatarCommand, cancellationToken);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
