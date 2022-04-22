using System.Collections.Generic;
using OWArcadeBackend.Dtos.Contributor.Profile.Game.Overwatch.Portraits;

namespace OWArcadeBackend.Dtos.Contributor.Profile.Game
{
    public class OverwatchDto
    {
        public List<Map> Maps { get; init; }
        public List<ArcadeMode> ArcadeModes { get; init; }
        public List<Hero> Heroes { get; init; }
    }
}