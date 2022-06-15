using OverwatchArcade.Domain.Models.Constants;

namespace OverwatchArcade.API.Dtos.Overwatch;

public class CreateDailyDto
{
    public Game Game { get; set; }
    public ICollection<CreateTileModeDto> TileModes { get; set; } = new List<CreateTileModeDto>();
}