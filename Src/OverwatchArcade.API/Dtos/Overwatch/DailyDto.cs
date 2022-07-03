using OverwatchArcade.API.Dtos.Contributor;

namespace OverwatchArcade.API.Dtos.Overwatch
{
    public class DailyDto
    {
        public bool IsToday { get; set; }
        public ICollection<TileModeDto> Modes { get; set; }
        public DateTime CreatedAt { get; set; }
        public ContributorDto Contributor { get; set; }
    }
}
