using System.Text.Json.Serialization;

namespace OverwatchArcade.API.Dtos.Discord
{
    public class DiscordToken
    {
        public DiscordToken(string accessToken, int expiresIn, string refreshToken, string scope, string tokenType)
        {
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
            RefreshToken = refreshToken;
            Scope = scope;
            TokenType = tokenType;
        }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        public string Scope { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}