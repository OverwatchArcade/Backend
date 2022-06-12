using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.API.Services.ConfigService;
using OverwatchArcade.API.Services.OverwatchService;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.Overwatch;
using OWArcadeBackend.Dtos.Overwatch;
using OWArcadeBackend.Services.ConfigService;

namespace OverwatchArcade.API.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OverwatchController : ControllerBase
    {
        private readonly IOverwatchService _overwatchService;
        private readonly IConfigService _configService;
        private readonly IMemoryCache _memoryCache;
        
        public OverwatchController(IOverwatchService overwatchService, IConfigService configService, IMemoryCache memoryCache)
        {
            _overwatchService = overwatchService ?? throw new ArgumentNullException(nameof(overwatchService));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }
        
        [Authorize]
        [HttpPost("submit")]
        public async Task<ActionResult<DailyDto>> PostOverwatchDaily(Daily daily)
        {
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not found in JWT"));
            var response = await _overwatchService.Submit(daily, Game.OVERWATCH, userId);
            return StatusCode(response.StatusCode, response);
        }
        
        [Authorize]
        [HttpPost("undo/{harddelete:bool}")]
        public async Task<ActionResult<Daily>> UndoOverwatchDaily(bool hardDelete)
        {
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not found in JWT"));
            var response = await _overwatchService.Undo(Game.OVERWATCH, userId, hardDelete);
            return StatusCode(response.StatusCode, response);
        }
        
        [EnableCors("OpenAPI")]
        [HttpGet("today")]
        public async Task<IActionResult> GetDaily()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.OverwatchDaily, out ServiceResponse<DailyDto> response))
            {
                response = await _overwatchService.GetDaily();
                Response.GetTypedHeaders().LastModified = response.Time;
            }
            else
            {
                Response.GetTypedHeaders().LastModified = DateTimeOffset.Now;
            }
            
            response.Time = DateTime.Now; // Overwrite cache datetime
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("event")]
        public IActionResult GetEvent()
        {
            var response = _memoryCache.Get<ServiceResponse<string>>(CacheKeys.ConfigOverwatchEvent);
            Response.GetTypedHeaders().LastModified = response.Time;
            response.Time = DateTime.Now; // Overwrite cache datetime
            
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("event/{overwatchEvent}")]
        public async Task<IActionResult> PostEvent(string overwatchEvent)
        {
            var response = await _configService.PostOverwatchEvent(overwatchEvent);
            return StatusCode(response.StatusCode, response);
        }
        
        [ResponseCache(Duration = 5)]
        [HttpGet("event/wallpaper")]
        public async Task<ActionResult> GetEventWallpaperUrl()
        {
            var response = await _configService.GetOverwatchEventWallpaper();
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("events")]
        public IActionResult GetEvents()
        {
            var response = _memoryCache.Get<ServiceResponse<string[]>>(CacheKeys.ConfigOverwatchEvents);
            Response.GetTypedHeaders().LastModified = response.Time;
            response.Time = DateTime.Now; // Overwrite cache datetime
            
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("arcademodes")]
        public IActionResult GetArcadeModes()
        {
            var response = _memoryCache.Get<ServiceResponse<List<ArcadeModeDto>>>(CacheKeys.OverwatchArcadeModes);
            Response.GetTypedHeaders().LastModified = response.Time;
            response.Time = DateTime.Now; // Overwrite cache datetime
            
            return StatusCode(response.StatusCode, response);
        }
        
        [Authorize]
        [HttpGet("labels")]
        public IActionResult GetLabels()
        {
            var response = _memoryCache.Get<ServiceResponse<List<Label>>>(CacheKeys.OverwatchLabels);
            Response.GetTypedHeaders().LastModified = response.Time;
            response.Time = DateTime.Now; // Overwrite cache datetime
            
            return StatusCode(response.StatusCode, response);
        }
    }
}
