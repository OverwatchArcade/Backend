using OverwatchArcade.Domain.Models.ContributorInformation;

namespace OverwatchArcade.API.Dtos.Contributor;

public class ContributorProfileDto
{
    public ContributorProfileDto(About personal, Socials social, Games game)
    {
        Personal = personal;
        Social = social;
        Game = game;
    }

    public About Personal { get; init; }
    public Socials Social { get; set; }
    public Games Game { get; init; }
}