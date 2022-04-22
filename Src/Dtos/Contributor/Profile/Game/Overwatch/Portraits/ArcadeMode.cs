using System;
using OWArcadeBackend.Models.Constants;

namespace OWArcadeBackend.Dtos.Contributor.Profile.Game.Overwatch.Portraits
{
    public class ArcadeMode
    {
        public string Name { get; set; }
        public string Image { get; set; }

        public string Url => Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwArcadeFolder + Image;
    }
}