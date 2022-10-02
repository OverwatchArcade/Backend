using AutoMapper;
using OverwatchArcade.Application.Common.Mappings;
using OverwatchArcade.Domain.Entities.Overwatch;

namespace OverwatchArcade.Application.Overwatch.Daily.Commands.CreateDaily;

public class CreateTileModeDto : IMapFrom<TileMode>
{
    public int ArcadeModeId { get; set; }
    public int TileId { get; set; }
    public int LabelId { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateTileModeDto, TileMode>();
    }
}