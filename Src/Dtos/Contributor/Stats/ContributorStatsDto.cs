#nullable enable
using System;
using System.Collections.Generic;

namespace OWArcadeBackend.Dtos.Contributor.Stats
{
    public class ContributorStatsDto
    {
        public int ContributionCount { get; set; }

        public DateTime? LastContributedAt { get; set; }
        public string? FavouriteContributionDay { get; set; }
        public IEnumerable<DateTime>? ContributionDays { get; set; }
    }
}