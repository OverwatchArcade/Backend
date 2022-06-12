﻿using OverwatchArcade.Domain.Models.Constants;

namespace OverwatchArcade.Domain.Models.ContributorProfile.Game.Overwatch.Portraits
{
    public class Map
    {
        public string Name { get; set; }
        public string Image { get; set; }

        public string Url => Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwMapsFolder + Image;
    }
}