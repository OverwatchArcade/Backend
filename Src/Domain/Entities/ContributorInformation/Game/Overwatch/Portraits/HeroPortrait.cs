using OverwatchArcade.Domain.Constants;

namespace OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits
{
    public class HeroPortrait
    {
        public string Name { get; set; }
        public string Image { get; set; }

        public string Url => Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwHeroesFolder + Image;
    }
}