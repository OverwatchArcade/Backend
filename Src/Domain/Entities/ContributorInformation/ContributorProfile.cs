using OverwatchArcade.Domain.Entities.ContributorInformation.Game;

namespace OverwatchArcade.Domain.Entities.ContributorInformation;

public class ContributorProfile
{
    public About Personal { get; init; }
    public Socials Social { get; set; }
    public OverwatchProfile Overwatch { get; init; }
}