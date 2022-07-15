using FluentValidation;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Discord;
using OverwatchArcade.API.Factories.Interfaces;
using OverwatchArcade.API.Utility;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscordApiClient _discordApiClient;
        private readonly IAuthenticationToken _authenticationToken;
        private readonly IValidator<DiscordLoginDto> _registerValidator;
        private readonly IServiceResponseFactory<string> _serviceResponseFactory;

        public AuthService(IUnitOfWork unitOfWork, IDiscordApiClient discordApiClient, IAuthenticationToken authenticationToken, IValidator<DiscordLoginDto> registerValidator, IServiceResponseFactory<string> serviceResponseFactory)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _discordApiClient = discordApiClient ?? throw new ArgumentNullException(nameof(discordApiClient));
            _authenticationToken = authenticationToken ?? throw new ArgumentNullException(nameof(authenticationToken));
            _registerValidator = registerValidator ?? throw new ArgumentNullException(nameof(registerValidator));
            _serviceResponseFactory = serviceResponseFactory ?? throw new ArgumentNullException(nameof(serviceResponseFactory));
        }

        public async Task<ServiceResponse<string>> RegisterAndLogin(string discordBearerToken, string redirectUri)
        {
            var discordToken = await _discordApiClient.GetDiscordToken(discordBearerToken, redirectUri);
            if (discordToken is null)
            {
                return _serviceResponseFactory.Error(500, "Couldn't get Discord Token");
            }
            var discordLoginDto = await _discordApiClient.MakeDiscordOAuthCall(discordToken.AccessToken);
            if (discordLoginDto is null)
            {
                return _serviceResponseFactory.Error(500, "Couldn't get Discord OAuth Token");
            }

            var contributor = _unitOfWork.ContributorRepository.FirstOrDefault(x => x.Email.Equals(discordLoginDto.Email));
            if (contributor is null)
            {
                var validatorResponse = await _registerValidator.ValidateAsync(discordLoginDto);
                if (!validatorResponse.IsValid)
                {
                    return _serviceResponseFactory.Error(500, string.Join(", ", validatorResponse.Errors.Select(x => x.ErrorMessage)));
                }
                contributor = await CreateContributor(discordLoginDto);
            }

            var jwtToken = _authenticationToken.CreateJwtToken(contributor);
            return _serviceResponseFactory.Create(jwtToken);
        }

        private async Task<Contributor> CreateContributor(DiscordLoginDto discordLoginDto)
        {
            var newContributor = new Contributor()
            {
                Email = discordLoginDto.Email,
                Username = discordLoginDto.Username,
                Group = ContributorGroup.Contributor
            };

            _unitOfWork.ContributorRepository.Add(newContributor);
            await _unitOfWork.Save();
            // Retrieve Contributor from DB So we have a generated UUID
            var contributor = await _unitOfWork.ContributorRepository.FirstOrDefaultASync(x => x.Email.Equals(discordLoginDto.Email));
            return contributor!;
        }
    }
}