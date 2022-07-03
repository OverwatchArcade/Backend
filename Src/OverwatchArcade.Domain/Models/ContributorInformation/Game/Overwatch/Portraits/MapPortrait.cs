using OverwatchArcade.Domain.Models.Constants;

namespace OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits
{
    public class MapPortrait
    {
        public string Name { get; set; }
        public string Image { get; set; }

        public string Url => Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwMapsFolder + Image;
    }
}