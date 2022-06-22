using OverwatchArcade.Domain.Models.Constants;

namespace OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits
{
    public class HeroPortrait
    {
        public string Name { get; set; }
        public string Image { get; set; }

        public string Url => Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwHeroesFolder + Image;
    }
}