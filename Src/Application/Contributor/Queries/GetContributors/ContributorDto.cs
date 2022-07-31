using OverwatchArcade.Application.Common.Mappings;
using OverwatchArcade.Application.Overwatch.Daily.Queries.GetDaily;

namespace OverwatchArcade.Application.Contributor.Queries.GetContributors;

public class ContributorDto : IMapFrom<Domain.Entities.Contributor>
{
    public string Username { get; set; }
    public string Avatar { get; set; }
    public DateTime RegisteredAt { get; set; }
    public ContributorStatsDto Stats { get; set; }
}