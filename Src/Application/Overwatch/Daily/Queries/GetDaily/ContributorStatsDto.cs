namespace OverwatchArcade.Application.Overwatch.Daily.Queries.GetDaily;

public class ContributorStatsDto
{
    public int Contributions { get; set; }
    public DateTime? LastContributedAt { get; set; }
    public string? FavouriteContributionDay { get; set; }
}