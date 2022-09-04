using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverwatchArcade.Application.Contributor.Commands.SaveAvatar;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllContributors()
        {
            var contributorDtos = await Mediator.Send(new GetContributorsQuery());
            return Ok(contributorDtos);
        }

        [HttpGet("{username}")]
        [Produces(typeof(ContributorDto))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContributorByUsername(GetContributorQuery getContributorQuery)
        {
            var contributorDto = await Mediator.Send(getContributorQuery);
            if (contributorDto is null)
            {
                return NotFound();
            }
            
            return Ok(contributorDto);
        }
        
        [Authorize]
        [HttpPost("profile")]
        public async Task<IActionResult> SaveProfile(SaveAvatarCommand saveAvatarCommand)
        {
            try
            {
                await Mediator.Send(saveAvatarCommand);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        
        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> SaveAvatar([FromForm] SaveAvatarCommand saveAvatarCommand)
        {
            try
            {
                await Mediator.Send(saveAvatarCommand);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
