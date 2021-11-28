using System.ComponentModel.DataAnnotations;

namespace OWArcadeBackend.Models.Overwatch
{
    public class TileMode
    {
        public int DailyId { get; set; }
        [Required]
        public int TileId { get; set; }
        [Required]
        public int ArcadeModeId { get; set; }
        public virtual ArcadeMode ArcadeMode { get; set; }
        [Required]
        public int LabelId { get; set; }
        public virtual Label Label { get; set; }
    }
}
