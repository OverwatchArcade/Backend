using System;
using System.Collections.Generic;

namespace OWArcadeBackend.Dtos
{
    public class DailyDto
    {
        public bool IsToday { get; set; } = false;
        public ICollection<TileModeDto> Modes { get; set; }
        public DateTime CreatedAt { get; set; }
        public ContributorDto Contributor { get; set; }
    }
}
