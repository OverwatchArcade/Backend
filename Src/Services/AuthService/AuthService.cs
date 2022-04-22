using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OWArcadeBackend.Dtos.Contributor;
using OWArcadeBackend.Dtos.Discord;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Constants;
using OWArcadeBackend.Persistence;
using OWArcadeBackend.Persistence.Repositories.Interfaces;
using OWArcadeBackend.Validators;
using OWArcadeBackend.Validators.Contributor;

namespace OWArcadeBackend.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IAuthRepository _authRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(IConfiguration configuration, IMapper mapper, IUnitOfWork unitOfWork,
            ILogger<AuthService> logger, IAuthRepository authRepository, IWebHostEnvironment hostEnvironment, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        private string CreateJwtToken(Contributor contributor)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, contributor.Id.ToString()),
                new(ClaimTypes.Name, contributor.Username),
                new(ClaimTypes.Role, contributor.Group.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Token").Value)
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(14),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private async Task<DiscordToken> GetDiscordToken(string code, string redirectUri)
        {
            var discordOAuthDetails = new List<KeyValuePair<string, string>>
            {
                new("client_id", _configuration.GetSection("Discord:clientId").Value),
                new("client_secret", _configuration.GetSection("Discord:clientSecret").Value),
                new("redirect_uri", redirectUri),
                new("grant_type", "authorization_code"),
                new("code", code)
            };
            
            using var content = new FormUrlEncodedContent(discordOAuthDetails);
            content.Headers.Clear();
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.PostAsync(DiscordConstants.DiscordTokenUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Couldn't exchange code for Discord Token");
                _logger.LogError(await response.Content.ReadAsStringAsync());
                throw new HttpRequestException();
            }

            return await response.Content.ReadFromJsonAsync<DiscordToken>();
        }

        private async Task<DiscordLoginDto> MakeDiscordOAuthCall(string token)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(DiscordConstants.UserInfoUrl);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Couldn't get Discord userinfo from bearer token {token}");
                _logger.LogError(await response.Content.ReadAsStringAsync());
                throw new HttpRequestException();
            }

            var responseObject = await response.Content.ReadFromJsonAsync<DiscordLoginDto>();

            return responseObject;
        }

        public async Task<ServiceResponse<string>> RegisterAndLogin(string discordBearerToken, string redirectUri)
        {
            var response = new ServiceResponse<string>();
            var discordToken = await GetDiscordToken(discordBearerToken, redirectUri);
            var discordLoginDto = await MakeDiscordOAuthCall(discordToken.AccessToken);
            var loginValidatorResult = await new LoginValidator(_unitOfWork).ValidateAsync(discordLoginDto);
            var registerValidatorResult = await new RegisterValidator(_unitOfWork).ValidateAsync(discordLoginDto);
            var contributor = _unitOfWork.ContributorRepository.SingleOrDefault(x => x.Email.Equals(discordLoginDto.Email));
            if (contributor == null)
            {
                if (!registerValidatorResult.IsValid)
                {
                    response.SetError(500, string.Join(", ", registerValidatorResult.Errors.Select(x => x.ErrorMessage)));
                    return response;
                }

                contributor = await CreateContributor(discordLoginDto);
            }
            else
            {
                if (!loginValidatorResult.IsValid)
                {
                    response.SetError(500, string.Join(", ", loginValidatorResult.Errors.Select(x => x.ErrorMessage)));
                    return response;
                }
            }
            response.Data = CreateJwtToken(contributor);
            return response;
        }

        private async Task<Contributor> CreateContributor(DiscordLoginDto discordLoginDto)
        {
            var newContributor = new Contributor()
            {
                Email = discordLoginDto.Email,
                Username = discordLoginDto.Username,
                Group = ContributorGroup.Contributor
            };

            _authRepository.Add(newContributor);
            await _unitOfWork.Save();

            var contributor = _unitOfWork.ContributorRepository.GetBy(x => x.Email.Equals(discordLoginDto.Email));
            return contributor;
        }

        public async Task<ServiceResponse<ContributorDto>> SaveProfile(ContributorProfileDto data, Guid userId)
        {
            var response = new ServiceResponse<ContributorDto>();
            var contributorProfileValidator = new ContributorProfileValidator(_unitOfWork);
            var result = await contributorProfileValidator.ValidateAsync(data);

            if (!result.IsValid)
            {
                response.SetError(500, string.Join(", ", result.Errors));
                return response;
            }

            try
            {
                Contributor contributor = _unitOfWork.ContributorRepository.Find(c => c.Id.Equals(userId)).Single();
                contributor.Profile = data;
                await _unitOfWork.Save();
                response.Data = _mapper.Map<ContributorDto>(contributor);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Couldn't save profile - " + e.Message);
                response.SetError(500, "Profile couldn't be updated");
            }

            return response;
        }

        public async Task<ServiceResponse<ContributorDto>> UploadAvatar(ContributorAvatarDto data, Guid userId)
        {
            var serviceResponse = new ServiceResponse<ContributorDto>();
            if (data.Avatar == null)
            {
                serviceResponse.SetError(500, "No data received");
                return serviceResponse;
            }

            ContributorAvatarValidator validator = new ContributorAvatarValidator();
            ValidationResult result = await validator.ValidateAsync(data.Avatar);

            if (!result.IsValid)
            {
                serviceResponse.SetError(500, string.Join(", ", result.Errors));
                return serviceResponse;
            }

            Contributor contributor = _unitOfWork.ContributorRepository.Find(c => c.Id.Equals(userId)).Single();
            
            var path = _hostEnvironment.WebRootPath + ImageConstants.ProfileFolder;
            var filename = Path.GetRandomFileName() + ".jpg";

            await CreateImage(data, path, filename, contributor, serviceResponse);
            await CompressImage(path, filename, serviceResponse);
            
            return serviceResponse;
        }

        private async Task CreateImage(ContributorAvatarDto data, string path, string filename, Contributor contributor, ServiceResponse<ContributorDto> serviceResponse)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                await using var fileStream = File.Create(path + filename);
                await data.Avatar.CopyToAsync(fileStream);
                await fileStream.FlushAsync();

                // Cleanup old image
                if (!contributor.Avatar.Equals("default.jpg"))
                {
                    File.Delete(path + contributor.Avatar);
                }

                contributor.Avatar = filename;
                await _unitOfWork.Save();
            }
            catch (Exception e)
            {
                _logger.LogWarning("Couldn't upload avatar - " + e.Message);
                serviceResponse.StatusCode = 500;
                serviceResponse.Message = "Avatar couldn't be uploaded";
            }
        }

        private async Task CompressImage(string path, string filename, ServiceResponse<ContributorDto> serviceResponse)
        {
            try
            {
                using var image = new MagickImage(path + filename);
                var size = new MagickGeometry(250, 250)
                {
                    IgnoreAspectRatio = true
                };

                image.Resize(size);
                await image.WriteAsync(path + filename);

                var optimizer = new ImageOptimizer();
                optimizer.Compress(path + filename);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Compression failed but avatar uploaded - " + e.Message);
                serviceResponse.StatusCode = 500;
                serviceResponse.Message = "Avatar uploaded but couldn't be compressed but uploaded correctly";
            }
        }
    }
}