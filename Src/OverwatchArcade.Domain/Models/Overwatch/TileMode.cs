namespace OverwatchArcade.Domain.Models.Overwatch
{
    public class TileMode
    {
        public int DailyId { get; set; }
        public int TileId { get; set; }
        public int ArcadeModeId { get; set; }
        public virtual ArcadeMode ArcadeMode { get; set; }
        public int LabelId { get; set; }
        public virtual Label Label { get; set; }
    }
}
