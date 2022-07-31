using OverwatchArcade.Application.Common.Mappings;
using OverwatchArcade.Domain.Entities.Overwatch;

namespace OverwatchArcade.Application.Overwatch.ArcadeModes.Commands;

public class ArcadeModeDto : IMapFrom<ArcadeMode>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Players { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
}