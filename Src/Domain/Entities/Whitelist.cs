using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Domain.Entities
{
    public class Whitelist
    {
        public string ProviderKey { get; set; }
        public LoginProviders Provider { get; set; }
    }
}