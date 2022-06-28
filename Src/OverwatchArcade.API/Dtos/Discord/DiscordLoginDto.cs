namespace OverwatchArcade.API.Dtos.Discord
{
    public class DiscordLoginDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public bool Verified { get; set; }
    }
}