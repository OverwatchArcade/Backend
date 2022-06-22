using OverwatchArcade.Domain.Models.ContributorInformation;

namespace OverwatchArcade.API.Dtos.Contributor;

public class ContributorProfileDto
{
    public IFormFile? Avatar { get; set; }
    public About Personal { get; init; }
    public Socials Social { get; set; }
    public Games Game { get; init; }
}