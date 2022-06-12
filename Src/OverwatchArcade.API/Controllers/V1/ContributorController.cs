using Microsoft.AspNetCore.Mvc;
using OverwatchArcade.API.Dtos;
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
            ServiceResponse<List<ContributorDto>> response = await _contributorService.GetAllContributors();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetContributorByUsername(string username)
        {
            ServiceResponse<ContributorDto> response = await _contributorService.GetContributorByUsername(username);
            return StatusCode(response.StatusCode, response);
        }
    }
}
