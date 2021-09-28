using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OWArcadeBackend.Models;
using OWArcadeBackend.Services.ConfigService;

namespace OWArcadeBackend.Controllers.V1.Contributor
{
    [Authorize]
    [Route("api/v1/contributor/[controller]")]
    [ApiController]
    public class ConfigController : Controller
    {
        private readonly IConfigService _configService;

        public ConfigController(IConfigService configService)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }
        
        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            ServiceResponse<IEnumerable<ConfigCountries>> response = await _configService.GetCountries();
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("overwatch/heroes")]
        public async Task<IActionResult> GetOverwatchHeroes()
        {
            ServiceResponse<IEnumerable<ConfigOverwatchHero>> response = await _configService.GetOverwatchHeroes();
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("overwatch/maps")]
        public async Task<IActionResult> GetOverwatchMaps()
        {
            ServiceResponse<IEnumerable<ConfigOverwatchMap>> response = await _configService.GetOverwatchMaps();
            return StatusCode(response.StatusCode, response);
        }
        
    }
}