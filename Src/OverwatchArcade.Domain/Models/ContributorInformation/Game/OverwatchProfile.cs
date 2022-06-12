using OverwatchArcade.Domain.Models.ContributorProfile.Game.Overwatch.Portraits;

namespace OverwatchArcade.Domain.Models.ContributorProfile.Game
{
    public class OverwatchProfile
    {
        public List<Map> Maps { get; init; }
        public List<ArcadeMode> ArcadeModes { get; init; }
        public List<Hero> Heroes { get; init; }
    }
}