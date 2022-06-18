using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Models.ContributorInformation.Personal;
using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.API.Controllers.V1.Contributor
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
            var response = _memoryCache.Get<ServiceResponse<IEnumerable<HeroPortrait>>>(CacheKeys.ConfigOverwatchHeroes);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("overwatch/maps")]
        public IActionResult GetOverwatchMaps()
        {
            var response = _memoryCache.Get<ServiceResponse<IEnumerable<MapPortrait>>>(CacheKeys.ConfigOverwatchMaps);
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