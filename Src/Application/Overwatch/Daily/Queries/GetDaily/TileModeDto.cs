using AutoMapper;
using OverwatchArcade.Application.Common.Mappings;
using OverwatchArcade.Domain.Entities.Overwatch;

namespace OverwatchArcade.Application.Overwatch.Daily.Queries.GetDaily;

public class TileModeDto : IMapFrom<TileMode>
{
    public string Name { get; set; }
    public string Players { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string Label { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<TileMode, TileModeDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ArcadeMode.Name))
            .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.ArcadeMode.Players))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ArcadeMode.Description))
            .ForMember(dest => dest.Image,opt => opt.MapFrom(src => src.ArcadeMode.Image))
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label.Value));
    }
}