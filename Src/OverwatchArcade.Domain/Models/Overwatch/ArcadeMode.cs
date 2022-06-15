using OverwatchArcade.Domain.Models.Constants;

namespace OverwatchArcade.Domain.Models.Overwatch
{
    public class ArcadeMode
    {
        public int Id { get; set; }
        public Game Game { get; set; }
        public string Name { get; set; }
        public string Players { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
