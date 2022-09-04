using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OverwatchArcade.Domain.Constants;
using OverwatchArcade.Persistence.ApiClient.Interfaces;
using OverwatchArcade.Persistence.Entities.Discord;

namespace OverwatchArcade.Persistence.ApiClient;


public class DiscordClient : IDiscordClient
{
    private readonly ILogger<DiscordClient> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public DiscordClient(ILogger<DiscordClient> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<DiscordToken?> GetDiscordToken(string code, string redirectUri)
    {
        var discordOAuthDetails = new List<KeyValuePair<string, string>>
        {
            new("client_id", _configuration.GetValue<string>("Discord:clientId")),
            new("client_secret", _configuration.GetValue<string>("Discord:clientSecret")),
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
    
    public async Task<DiscordLogin?> MakeDiscordOAuthCall(string token)
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

        return await response.Content.ReadFromJsonAsync<DiscordLogin>();
    }
}