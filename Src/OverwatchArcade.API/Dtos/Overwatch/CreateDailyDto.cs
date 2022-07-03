namespace OverwatchArcade.API.Dtos.Overwatch;

public class CreateDailyDto
{
    public ICollection<CreateTileModeDto> TileModes { get; set; } = new List<CreateTileModeDto>();
}