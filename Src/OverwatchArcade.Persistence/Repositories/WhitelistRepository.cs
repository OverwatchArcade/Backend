using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Persistence.Repositories.Interfaces;

namespace OverwatchArcade.Persistence.Repositories
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