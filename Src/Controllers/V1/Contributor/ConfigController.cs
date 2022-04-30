using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OWArcadeBackend.Dtos.Contributor.Profile.Game.Overwatch.Portraits;
using OWArcadeBackend.Dtos.Contributor.Profile.Personal;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Constants;

namespace OWArcadeBackend.Controllers.V1.Contributor
{
    [Authorize]
    [Route("api/v1/contributor/[controller]")]
    [ApiController]
    public class ConfigController : Controller
    {
        private readonly IMemoryCache _memoryCache;

        public ConfigController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        [HttpGet("countries")]
        public IActionResult GetCountries()
        {
            var response = _memoryCache.Get<ServiceResponse<IEnumerable<Country>>>(CacheKeys.Countries);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("overwatch/heroes")]
        public IActionResult GetOverwatchHeroes()
        {
            var response = _memoryCache.Get<ServiceResponse<IEnumerable<Hero>>>(CacheKeys.ConfigOverwatchHeroes);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("overwatch/maps")]
        public IActionResult GetOverwatchMaps()
        {
            var response = _memoryCache.Get<ServiceResponse<IEnumerable<Map>>>(CacheKeys.ConfigOverwatchMaps);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("overwatch/arcademodes")]
        public IActionResult GetOverwatchArcadeModes()
        {
            var response = _memoryCache.Get<ServiceResponse<IEnumerable<ArcadeMode>>>(CacheKeys.ConfigOverwatchArcadeModes);
            return StatusCode(response.StatusCode, response);
        }

    }
}