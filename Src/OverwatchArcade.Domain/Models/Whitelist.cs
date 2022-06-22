using OverwatchArcade.Domain.Models.Constants;

namespace OverwatchArcade.Domain.Models
{
    public class Whitelist
    {
        public string ProviderKey { get; set; }
        public SocialProviders Provider { get; set; }
    }
}