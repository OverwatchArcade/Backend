using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Domain.Models.ContributorInformation.Game;

namespace OverwatchArcade.API.Dtos.Contributor;

public class ContributorProfileDto
{
    public About Personal { get; init; }
    public Socials Social { get; set; }
    public OverwatchProfile Overwatch { get; set; }
}