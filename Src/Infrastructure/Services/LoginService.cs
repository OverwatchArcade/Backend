using MediatR;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Application.Contributor.Commands.CreateContributor;
using OverwatchArcade.Application.Contributor.Queries.GetContributor;
using OverwatchArcade.Application.Whitelist.Queries;
using OverwatchArcade.Domain.Enums;
using OverwatchArcade.Persistence.ApiClient.Interfaces;
using OverwatchArcade.Persistence.Services.Interfaces;

namespace OverwatchArcade.Persistence.Services;

public class LoginService : ILoginService
{
    private readonly IMediator _mediator;
    private readonly IGenerateJwt _generateJwt;
    private readonly IDiscordClient _discordClient;

    public LoginService(IMediator mediator, IGenerateJwt generateJwt, IDiscordClient discordClient)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _generateJwt = generateJwt ?? throw new ArgumentNullException(nameof(generateJwt));
        _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
    }

    /// <summary>
    /// Creates a contributor if it doesn't exist and is whitelisted
    /// Returns generated JWT Token
    /// </summary>
    /// <returns></returns>
    public async Task<string> Login(string discordAccessToken)
    {
        var discordLoginDto = await _discordClient.MakeDiscordOAuthCall(discordAccessToken);
        if (discordLoginDto is null)
        {
            throw new HttpRequestException("Couldn't Discord OAuth token");
        }
        
        var contributor = await _mediator.Send(new GetContributorQuery()
        {
            Email = discordLoginDto.Email
        });
        
        if (contributor is null)
        {
            var isWhitelisted = await _mediator.Send(new GetWhitelistQuery(LoginProviders.Discord.ToString(), discordLoginDto.Id));
            if (isWhitelisted is false)
            {
                throw new HttpRequestException("User is not whitelisted");
            }
            
            contributor = await _mediator.Send(new CreateContributorCommand()
            {
                Username = discordLoginDto.Username,
                Email = discordLoginDto.Email
            });
        }
         
        return _generateJwt.CreateToken(contributor);
    }
}