using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OWArcadeBackend.Models.Overwatch
{
    public class Daily
    { 
        public int Id { get; set; }
        public Game Game { get; set; }
        [Required]
        public virtual ICollection<TileMode> TileModes { get; set; }
        public Guid ContributorId { get; set; }
        public virtual Contributor Contributor { get; set; }
        public bool MarkedOverwrite { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
