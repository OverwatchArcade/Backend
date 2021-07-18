using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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
