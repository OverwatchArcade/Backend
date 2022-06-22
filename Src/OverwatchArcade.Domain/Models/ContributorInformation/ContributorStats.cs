namespace OverwatchArcade.Domain.Models.ContributorInformation;

public class ContributorStats
{
    public int ContributionCount { get; set; }

    public DateTime? LastContributedAt { get; set; }
    public string? FavouriteContributionDay { get; set; }
    public IEnumerable<DateTime>? ContributionDays { get; set; }
}