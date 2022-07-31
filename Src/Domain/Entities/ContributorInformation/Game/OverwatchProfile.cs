using OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits;

namespace OverwatchArcade.Domain.Entities.ContributorInformation.Game
{
    public class OverwatchProfile
    {
        public List<MapPortrait> Maps { get; init; }
        public List<ArcadeModePortrait> ArcadeModes { get; init; }
        public List<HeroPortrait> Heroes { get; init; }
    }
}