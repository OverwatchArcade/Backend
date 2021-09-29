using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Services.ConfigService;
using OWArcadeBackend.Services.OverwatchService;

namespace OWArcadeBackend.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OverwatchController : ControllerBase
    {
        private readonly IOverwatchService _overwatchService;
        private readonly IConfigService _configService;
        private readonly IMemoryCache _cache;
        
        public OverwatchController(IOverwatchService overwatchService, IConfigService configService, IMemoryCache cache)
        {
            _overwatchService = overwatchService ?? throw new ArgumentNullException(nameof(overwatchService));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
        
        [Authorize]
        [HttpPost("submit")]
        public async Task<ActionResult<DailyDto>> PostOverwatchDaily(Daily daily)
        {
            Guid userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            ServiceResponse<DailyDto> response = await _overwatchService.Submit(daily, Game.OVERWATCH, userId);
            return StatusCode(response.StatusCode, response);
        }
        
        [Authorize]
        [HttpPost("undo/{harddelete:bool}")]
        public async Task<ActionResult<Daily>> UndoOverwatchDaily(bool hardDelete)
        {
            Guid userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            ServiceResponse<DailyDto> response = await _overwatchService.Undo(Game.OVERWATCH, userId, hardDelete);
            return StatusCode(response.StatusCode, response);
        }
        
        [EnableCors("OpenAPI")]
        [HttpGet("today")]
        public async Task<IActionResult> GetDaily()
        {
            if (!_cache.TryGetValue(CacheKeys.OverwatchDaily, out ServiceResponse<DailyDto> response))
            {
                response = await _overwatchService.GetDaily();
            }
            
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("event")]
        public async Task<IActionResult> GetEvent()
        {
            if (!_cache.TryGetValue(CacheKeys.OverwatchEvent, out ServiceResponse<string> response))
            {
                response = await _configService.GetCurrentOverwatchEvent();
            }
            
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
            if (!_cache.TryGetValue(CacheKeys.OverwatchEvents, out ServiceResponse<string[]> response))
            {
                response = _configService.GetOverwatchEvents();
            }
            
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("arcademodes")]
        public IActionResult GetArcadeModes()
        {
            if (!_cache.TryGetValue(CacheKeys.OverwatchArcademodes, out ServiceResponse<List<ArcadeModeDto>> response))
            {
                response = _overwatchService.GetArcadeModes();
            }
            
            return StatusCode(response.StatusCode, response);
        }
        
        [Authorize]
        [HttpGet("labels")]
        public IActionResult GetLabels()
        {
            if (!_cache.TryGetValue(CacheKeys.OverwatchLabels, out ServiceResponse<List<Label>> response))
            {
                response = _overwatchService.GetLabels();
            }
            
            return StatusCode(response.StatusCode, response);
        }
    }
}
