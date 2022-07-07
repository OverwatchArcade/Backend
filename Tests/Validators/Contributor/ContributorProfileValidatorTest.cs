using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;
using Newtonsoft.Json.Linq;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Validators.Contributor;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Domain.Models.ContributorInformation.Game;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Models.ContributorInformation.Personal;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Validators.Contributor
{
    public class ContributorProfileValidatorTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        private readonly ContributorProfileValidator _contributorProfileValidator;
        private ContributorProfileDto _contributorProfile;

        public ContributorProfileValidatorTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            PrepareMock();

            _contributorProfileValidator = new ContributorProfileValidator(_unitOfWorkMock.Object);
        }

        private void PrepareMock()
        {
            _contributorProfile = new ContributorProfileDto()
            {
                Overwatch = new OverwatchProfile
                {
                    ArcadeModes = new List<ArcadeModePortrait>
                    {
                        new()
                        {
                            Name = "Total Mayhem",
                            Image = "image.jpg"
                        }
                    },
                    Maps = new List<MapPortrait>
                    {
                        new()
                        {
                            Name = "Ayutthaya",
                            Image = "EEA8BFCDB3B0890541E285A06B2576D1.jpg",
                        }
                    },
                    Heroes = new List<HeroPortrait>
                    {
                        new()
                        {
                            Name = "Soldier-76",
                            Image = "EEA8BFCDB3B0890541E285A06B2576D1.jpg"
                        }
                    }
                },
                Personal = new About()
                {
                    Text = "I like writing Unit Tests",
                    Country = new Country
                    {
                        Name = "Netherlands",
                        Code = "NL"
                    }
                },
                Social = new Socials()
                {
                    Battlenet = "battlenet",
                    Discord = "Discord",
                    Steam = "steam",
                    Twitter = "Twitter"
                }
            };

            var mapConfigDatabaseResult = new Config()
            {
                Value = "",
                JsonValue = JArray.Parse("[{\"Name\":\"Ayutthaya\",\"Image\":\"EEA8BFCDB3B0890541E285A06B2576D1.jpg\"}]")
            };

            var heroesConfigDatabaseResult = new Config()
            {
                Value = "",
                JsonValue = JArray.Parse("[{\"Name\":\"Soldier-76\",\"Image\":\"EEA8BFCDB3B0890541E285A06B2576D1.jpg\"}]")
            };

            var arcademodesDatabaseResult = new List<ArcadeMode>()
            {
                new()
                {
                    Id = 1,
                    Name = "Total Mayhem",
                    Players = "6v6",
                    Image = "image.jpg",
                }
            };

            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Find(It.IsAny<Expression<Func<ArcadeMode, bool>>>())).Returns(arcademodesDatabaseResult);
            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefault(y => y.Key == ConfigKeys.OwMaps.ToString())).Returns(mapConfigDatabaseResult);
            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefault(y => y.Key == ConfigKeys.OwHeroes.ToString())).Returns(heroesConfigDatabaseResult);
        }

        [Fact]
        public void Constructor()
        {
            var constructor = new ContributorProfileValidator(_unitOfWorkMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void ConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new ContributorProfileValidator(
                null
            ));
        }

        [Fact]
        public void ProfileValidator_Success()
        {
            // Arrange

            // Act
            var result = _contributorProfileValidator.Validate(_contributorProfile);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ProfileValidator_Invalid()
        {
            // Arrange
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Find(It.IsAny<Expression<Func<ArcadeMode, bool>>>())).Returns(new List<ArcadeMode>());
            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefault(y => y.Key == ConfigKeys.OwMaps.ToString())).Returns((Config)null);
            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefault(y => y.Key == ConfigKeys.OwHeroes.ToString())).Returns((Config)null);

            // Act
            var result = new ContributorProfileValidator(_unitOfWorkMock.Object).Validate(_contributorProfile);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(3, result.Errors.Count);
            Assert.Equal("Overwatch Arcade Total Mayhem doesn't seem to be valid", result.Errors[0].ErrorMessage);
            Assert.Equal("Overwatch Hero Soldier-76 doesn't seem to be valid", result.Errors[1].ErrorMessage);
            Assert.Equal("Overwatch Map Ayutthaya doesn't seem to be valid", result.Errors[2].ErrorMessage);
        }
    }
}