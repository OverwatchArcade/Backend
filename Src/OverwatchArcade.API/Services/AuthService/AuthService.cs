using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Discord;
using OverwatchArcade.API.Validators;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Persistence;
using OverwatchArcade.Persistence.Repositories.Interfaces;

namespace OverwatchArcade.API.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;
        private readonly IAuthRepository _authRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(IConfiguration configuration, IUnitOfWork unitOfWork,
            ILogger<AuthService> logger, IAuthRepository authRepository, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
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
            var response = await client.PostAsync(DiscordConstants.DiscordTokenUrl, content);

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
    }
}