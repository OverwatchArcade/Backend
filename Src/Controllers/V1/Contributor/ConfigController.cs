﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
            var response = _memoryCache.Get<ServiceResponse<IEnumerable<ConfigCountries>>>(CacheKeys.Countries);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("overwatch/heroes")]
        public IActionResult GetOverwatchHeroes()
        {
            var response = _memoryCache.Get<ServiceResponse<IEnumerable<ConfigOverwatchHero>>>(CacheKeys.OverwatchHeroes);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("overwatch/maps")]
        public IActionResult GetOverwatchMaps()
        {
            var response = _memoryCache.Get<ServiceResponse<IEnumerable<ConfigOverwatchMap>>>(CacheKeys.OverwatchMaps);
            return StatusCode(response.StatusCode, response);
        }
        
    }
}