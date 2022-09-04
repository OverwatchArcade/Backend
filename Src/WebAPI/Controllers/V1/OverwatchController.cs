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

namespace WebAPI.Controllers.V1
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
        public async Task<ActionResult<DailyDto>> PostOverwatchDaily(CreateDailyCommand createDailyCommand)
        {
            var response = await Mediator.Send(createDailyCommand);
            return Ok(response);
        }
        
        [Authorize]
        [HttpPost("undo/hard")]
        public async Task<IActionResult> HardUndoOverwatchDaily()
        {
            await Mediator.Send(new DeleteDailyCommand());
            return NoContent();
        }
        
        [Authorize]
        [HttpPost("undo/soft")]
        public async Task<IActionResult> SoftUndoOverwatchDaily()
        {
            await Mediator.Send(new SoftDeleteDailyCommand());
            return NoContent();
        }
        
        [EnableCors("OpenAPI")]
        [HttpGet("today")]
        public async Task<IActionResult> GetDaily()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.OverwatchDaily, out DailyDto dailyDto))
            {
                dailyDto = await Mediator.Send(new GetDailyQuery());
            }

            return Ok(dailyDto);
        }
        
        [HttpGet("event")]
        public IActionResult GetEvent()
        {
            var currentEvent = _memoryCache.Get<string>(CacheKeys.ConfigOverwatchEvent);
            return Ok(currentEvent);
        }

        [Authorize]
        [HttpPost("event/{overwatchEvent}")]
        public async Task<IActionResult> PostEvent(string overwatchEvent)
        {
            await Mediator.Send(new PostOverwatchEventCommand(overwatchEvent));
            return Ok();
        }
        
        [ResponseCache(Duration = 5)]
        [HttpGet("event/wallpaper")]
        public async Task<ActionResult> GetEventWallpaperUrl()
        {
            var wallpaper = await Mediator.Send(new GetWallpaperQuery());
            return Ok(wallpaper);
        }
        
        [HttpGet("events")]
        public IActionResult GetEvents()
        {
            var events = _memoryCache.Get<string[]>(CacheKeys.ConfigOverwatchEvents);
            return Ok(events);
        }

        [Authorize]
        [HttpGet("arcademodes")]
        public IActionResult GetArcadeModes()
        {
            var arcadeModeDtos = _memoryCache.Get<ICollection<ArcadeModeDto>>(CacheKeys.OverwatchArcadeModesDtos);
            return Ok(arcadeModeDtos);
        }
        
        [Authorize]
        [HttpGet("labels")]
        public IActionResult GetLabels()
        {
            var labels = _memoryCache.Get<List<Label>>(CacheKeys.OverwatchLabels);
            return Ok(labels);
        }
    }
}
