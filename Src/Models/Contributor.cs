using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using OWArcadeBackend.Dtos.Contributor;
using OWArcadeBackend.Models.Constants;

namespace OWArcadeBackend.Models
{
    public class Contributor
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public ContributorGroup Group { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        #nullable enable
        public ContributorProfile? Profile { get; set; }
        [NotMapped] public ContributorStats? Stats { get; set; }
    }
    public class ContributorStats
    {
        public int ContributionCount { get; set; }
        public DateTime? LastContributedAt { get; set; }
        public string? FavouriteContributionDay { get; set; }
        public IEnumerable<DateTime>? ContributionDays { get; set; }
    }

    public class ContributorProfile
    {
        public ContributorProfileAbout? Personal { get; set; }
        public ContributorProfileSocials? Social { get; set; }
        public ContributorProfileGame? Game { get; set; }
    }

    public class ContributorProfileAbout
    {
        public string? About { get; set; }
        public ConfigCountries? Country { get; set; }
    }

    public class ContributorProfileSocials
    {
        public string? Discord { get; set; }
        public string? Battlenet { get; set; }
        public string? Steam { get; set; }
        public string? Twitter { get; set; }
    }

    public class ContributorProfileGame
    {
        public ContributorProfileGameOverwatch? Overwatch { get; set; }
    }

    public class ContributorProfileGameOverwatch
    {
        public List<ConfigOverwatchMap>? Maps { get; set; }
        public List<ArcadeModeSettingDto>? ArcadeModes { get; set; }
        public List<ConfigOverwatchHero>? Heroes { get; set; }
    }
}