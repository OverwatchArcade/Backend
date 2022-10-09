using OverwatchArcade.Domain.Constants;

namespace OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits
{
    public class ArcadeModePortrait : Portrait
    {
        public ArcadeModePortrait(string name, string image) : base(name, image)
        {
        }
        
        public string Url => Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwArcadeFolder + Image;
    }
}