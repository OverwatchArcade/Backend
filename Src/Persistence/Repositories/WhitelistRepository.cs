using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence.Repositories.Interfaces;

namespace OWArcadeBackend.Persistence.Repositories
{
    public class WhitelistRepository : Repository<Whitelist>, IWhitelistRepository
    {
        public WhitelistRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public bool IsDiscordWhitelisted(string providerKey)
        {
            return MUnitOfWork.WhitelistRepository.Exists(
                x => x.ProviderKey.Equals(providerKey) && x.Provider.Equals(SocialProviders.Discord)
            );
        }
    }
}