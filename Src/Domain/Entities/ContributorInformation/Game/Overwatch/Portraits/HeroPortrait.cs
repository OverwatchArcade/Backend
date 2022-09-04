﻿using OverwatchArcade.Domain.Constants;

namespace OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits
{
    public class HeroPortrait : Portrait
    {
        public string Url => Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwHeroesFolder + Image;
    }
}