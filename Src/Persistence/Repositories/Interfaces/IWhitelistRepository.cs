using OWArcadeBackend.Models;

namespace OWArcadeBackend.Persistence.Repositories.Interfaces
{
    public interface IWhitelistRepository : IRepository<Whitelist>
    {
        public bool IsDiscordWhitelisted(string providerKey);
    }
}