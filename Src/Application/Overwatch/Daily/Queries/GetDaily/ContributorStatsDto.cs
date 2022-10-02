using AutoMapper;
using OverwatchArcade.Application.Common.Mappings;
using OverwatchArcade.Domain.Entities.ContributorInformation;

namespace OverwatchArcade.Application.Overwatch.Daily.Queries.GetDaily;

public class ContributorStatsDto : IMapFrom<ContributorStats>
{
    public int Contributions { get; set; }
    public DateTime? LastContributedAt { get; set; }
    public string? FavouriteContributionDay { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<ContributorStats, ContributorStatsDto>()
            .ForMember(cs => cs.Contributions, opt => opt.MapFrom(s => s.ContributionCount));
    }
}