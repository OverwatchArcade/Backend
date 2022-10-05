using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.Application.Config.Commands.PostOverwatchEvent;
using OverwatchArcade.Application.Config.Queries.GetWallpaper;
using OverwatchArcade.Application.Overwatch.ArcadeModes.Commands;
using OverwatchArcade.Application.Overwatch.Daily.Commands.CreateDaily;
using OverwatchArcade.Application.Overwatch.Daily.Commands.DeleteDaily;
using OverwatchArcade.Application.Overwatch.Daily.Commands.SoftDeleteDaily;
using OverwatchArcade.Application.Overwatch.Daily.Queries.GetDaily;
using OverwatchArcade.Domain.Entities.Overwatch;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.WebAPI.Controllers.V2
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OverwatchController : ApiControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        public OverwatchController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        [Authorize]
        [HttpPost("submit")]
        [ProducesResponseType(typeof(CreateDailyCommand), StatusCodes.Status200OK)]
        public async Task<ActionResult> PostOverwatchDaily(CreateDailyCommand createDailyCommand)
        {
            await Mediator.Send(createDailyCommand);
            return Ok();
        }
        
        [Authorize]
        [HttpDelete("undo/hard")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> HardUndoOverwatchDaily()
        {
            await Mediator.Send(new DeleteDailyCommand());
            return NoContent();
        }
        
        [Authorize]
        [HttpDelete("undo/soft")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SoftUndoOverwatchDaily()
        {
            await Mediator.Send(new SoftDeleteDailyCommand());
            return NoContent();
        }
        
        [EnableCors("OpenAPI")]
        [HttpGet("today")]
        [ProducesResponseType(typeof(DailyDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDaily()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.OverwatchDaily, out DailyDto dailyDto))
            {
                dailyDto = await Mediator.Send(new GetDailyQuery());
                _memoryCache.Set(CacheKeys.OverwatchDaily, dailyDto, DateTimeOffset.UtcNow.AddSeconds(30));
            }

            return Ok(dailyDto);
        }
        
        [HttpGet("event")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetEvent()
        {
            var currentEvent = _memoryCache.Get<string>(CacheKeys.ConfigOverwatchEvent);
            return Ok(currentEvent);
        }

        [Authorize]
        [HttpPost("event/{overwatchEvent}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostEvent(string overwatchEvent)
        {
            await Mediator.Send(new PostOverwatchEventCommand(overwatchEvent));
            return Ok();
        }
        
        [ResponseCache(Duration = 5)]
        [HttpGet("event/wallpaper")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetEventWallpaperUrl()
        {
            var wallpaper = await Mediator.Send(new GetWallpaperQuery());
            return Ok(wallpaper);
        }
        
        [HttpGet("events")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetEvents()
        {
            var events = _memoryCache.Get<string[]>(CacheKeys.ConfigOverwatchEvents);
            return Ok(events);
        }

        [Authorize]
        [HttpGet("arcademodes")]
        [ProducesResponseType(typeof(ICollection<ArcadeModeDto>), StatusCodes.Status200OK)]
        public IActionResult GetArcadeModes()
        {
            var arcadeModeDtos = _memoryCache.Get<ICollection<ArcadeModeDto>>(CacheKeys.OverwatchArcadeModesDtos);
            return Ok(arcadeModeDtos);
        }
        
        [Authorize]
        [HttpGet("labels")]
        [ProducesResponseType(typeof(List<Label>), StatusCodes.Status200OK)]
        public IActionResult GetLabels()
        {
            var labels = _memoryCache.Get<List<Label>>(CacheKeys.OverwatchLabels);
            return Ok(labels);
        }
    }
}
