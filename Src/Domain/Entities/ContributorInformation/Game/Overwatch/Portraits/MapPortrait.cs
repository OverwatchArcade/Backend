using OverwatchArcade.Domain.Constants;

namespace OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits
{
    public class MapPortrait : Portrait
    {
        public MapPortrait(string name, string image) : base(name, image)
        {
        }
        
        public string Url => Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwMapsFolder + Image;
    }
}