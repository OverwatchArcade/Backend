using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.Application.Overwatch.ArcadeModes.Commands;
using OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Entities.ContributorInformation.Personal;
using OverwatchArcade.Domain.Entities.Overwatch;
using OverwatchArcade.Domain.Enums;

namespace WebAPI.Controllers.V1.Contributor
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
        [Produces(typeof(IEnumerable<Country>))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCountries()
        {
            var countries = _memoryCache.Get<IEnumerable<Country>>(CacheKeys.Countries);
            return Ok(countries);
        }
        
        [HttpGet("overwatch/heroes")]
        [Produces(typeof(IEnumerable<HeroPortrait>))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetOverwatchHeroes()
        {
            var heroPortraits = _memoryCache.Get<IEnumerable<HeroPortrait>>(CacheKeys.ConfigOverwatchHeroes);
            return Ok(heroPortraits);
        }
        
        [HttpGet("overwatch/maps")]
        [Produces(typeof(IEnumerable<MapPortrait>))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetOverwatchMaps()
        {
            var mapPortraits = _memoryCache.Get<IEnumerable<MapPortrait>>(CacheKeys.ConfigOverwatchMaps);
            return Ok(mapPortraits);
        }
        
        [HttpGet("overwatch/arcademodes")]
        [Produces(typeof(IEnumerable<ArcadeModeDto>))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetOverwatchArcadeModes()
        {
            var arcadeModes = _memoryCache.Get<IEnumerable<ArcadeMode>>(CacheKeys.ConfigOverwatchArcadeModes);
            return Ok(arcadeModes);
        }
    }
}