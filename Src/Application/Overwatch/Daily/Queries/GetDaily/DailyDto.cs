using AutoMapper;
using OverwatchArcade.Application.Common.Mappings;
using OverwatchArcade.Application.Contributor.Queries.GetContributors;

namespace OverwatchArcade.Application.Overwatch.Daily.Queries.GetDaily
{
    public class DailyDto : IMapFrom<Domain.Entities.Overwatch.Daily>
    {
        public bool IsToday { get; set; }
        public ICollection<TileModeDto> Modes { get; set; } = new List<TileModeDto>();
        public DateTime CreatedAt { get; set; }
        public ContributorDto Contributor { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.Overwatch.Daily, DailyDto>()
                .ForMember(dest => dest.Modes, opt => opt.MapFrom(src => src.TileModes));
        }
    }
}
