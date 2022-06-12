﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Services.AuthService;
using OverwatchArcade.API.Services.ContributorService;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Domain.Models.ContributorProfile;

namespace OverwatchArcade.API.Controllers.V1.Contributor
{
    [ApiController]
    [Route("api/v1/contributor/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IContributorService _contributorService;

        public AuthController(IAuthService authService, IContributorService contributorService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _contributorService = contributorService ?? throw new ArgumentNullException(nameof(contributorService));
        }
        
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string code, string redirectUri)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(redirectUri))
            {
                return BadRequest();
            }
            
            ServiceResponse<string> response = await _authService.RegisterAndLogin(code, redirectUri);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            return Ok();
        }

        [Authorize]
        [HttpGet("Info")]
        public async Task<IActionResult> Info()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? throw new Exception("User not found in JWT");
            ServiceResponse<ContributorDto> response = await _contributorService.GetContributorByUsername(username);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("profile")]
        public async Task<IActionResult> SaveProfile(ContributorProfile contributorProfile)
        {
            Guid userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not found in JWT"));
            ServiceResponse<ContributorDto> response = await _authService.SaveProfile(contributorProfile, userId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] ContributorAvatarDto file)
        {
            Guid userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not found in JWT"));
            ServiceResponse<ContributorDto> response = await _authService.UploadAvatar(file, userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}