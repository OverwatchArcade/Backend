using System;
using System.ComponentModel.DataAnnotations.Schema;
using OWArcadeBackend.Dtos.Contributor;
using OWArcadeBackend.Dtos.Contributor.Profile;
using OWArcadeBackend.Dtos.Contributor.Stats;
using OWArcadeBackend.Models.Constants;

namespace OWArcadeBackend.Models
{
    public class Contributor
    {
        public Guid Id { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
        public string Avatar { get; set; }
        public ContributorGroup Group { get; init; }
        public DateTime RegisteredAt { get; init; }
        public ContributorProfileDto Profile { get; set; }
        [NotMapped] public ContributorStatsDto Stats { get; set; }
    }
}