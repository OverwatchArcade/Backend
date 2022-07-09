using System.Text.Json.Serialization;

namespace OverwatchArcade.API.Dtos.Contributor;

public class ContributorStatsDto
{
    public int ContributionCount { get; set; }

    public DateTime? LastContributedAt { get; set; }
    public string? FavouriteContributionDay { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<DateTime>? ContributionDays { get; set; }
}