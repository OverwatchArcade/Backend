﻿using OverwatchArcade.Domain.Models;

namespace OverwatchArcade.Persistence.Persistence.Repositories.Interfaces
{
    public interface IWhitelistRepository : IRepository<Whitelist>
    {
        public bool IsDiscordWhitelisted(string providerKey);
    }
}