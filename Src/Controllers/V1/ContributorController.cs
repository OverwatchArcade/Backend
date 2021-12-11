using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Dtos.Contributor;
using OWArcadeBackend.Models;
using OWArcadeBackend.Services.ContributorService;

namespace OWArcadeBackend.Controllers.V1
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
