﻿namespace OverwatchArcade.Domain.Entities.Overwatch
{
    public class Daily
    { 
        public int Id { get; set; }
        public virtual IEnumerable<TileMode> TileModes { get; set; }
        public Guid ContributorId { get; set; }
        public virtual Contributor Contributor { get; set; }
        public bool MarkedOverwrite { get; set; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }

}
