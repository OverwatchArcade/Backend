namespace OverwatchArcade.API.Dtos.Discord
{
    public class DiscordLoginDto
    {
        public DiscordLoginDto(string id, string username, string avatar, string email, bool verified)
        {
            Id = id;
            Username = username;
            Avatar = avatar;
            Email = email;
            Verified = verified;
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public bool Verified { get; set; }
    }
}