using OverwatchArcade.Domain.Constants;

namespace OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits
{
    public class MapPortrait
    {
        public string Name { get; set; }
        public string Image { get; set; }

        public string Url => Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwMapsFolder + Image;
    }
}