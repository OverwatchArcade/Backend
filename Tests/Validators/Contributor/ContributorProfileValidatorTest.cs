using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;
using Newtonsoft.Json.Linq;
using OWArcadeBackend.Dtos.Contributor;
using OWArcadeBackend.Dtos.Contributor.Profile;
using OWArcadeBackend.Dtos.Contributor.Profile.Game;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Constants;
using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Persistence;
using OWArcadeBackend.Validators.Contributor;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Validators.Contributor
{
    public class ContributorProfileValidatorTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private ContributorProfileDto _contributorProfile;

        public ContributorProfileValidatorTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            PrepareMock();
        }

        private void PrepareMock()
        {
            _contributorProfile = new ContributorProfileDto()
            {
                Game = new ()
                {
                    Overwatch = new()
                    {
                        ArcadeModes = new()
                        {
                            new()
                            {
                                Name = "Total Mayhem",
                                Image = "image.jpg"
                            }
                        },
                        Maps = new()
                        {
                            new()
                            {
                                Name = "Ayutthaya",
                                Image = "EEA8BFCDB3B0890541E285A06B2576D1.jpg",
                            }
                        },
                        Heroes = new()
                        {
                            new()
                            {
                                Name = "Soldier-76",
                                Image = "EEA8BFCDB3B0890541E285A06B2576D1.jpg"
                            }
                        }
                    }
                },
                Personal = new AboutDto()
                {
                    About = "I like writing Unit Tests",
                    Country = new()
                    {
                        Name = "Netherlands",
                        Code = "NL"
                    }
                },
                Social = new SocialsDto()
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
                    Game = Game.OVERWATCH
                }
            };
            
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Find(It.IsAny<Expression<Func<ArcadeMode,bool>>>())).Returns(arcademodesDatabaseResult);
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefault(y => y.Key == ConfigKeys.OW_MAPS.ToString())).Returns(mapConfigDatabaseResult);
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefault(y => y.Key == ConfigKeys.OW_HEROES.ToString())).Returns(heroesConfigDatabaseResult);
        }

        [Fact]
        public void TestConstructor()
        {
            var constructor = new ContributorProfileValidator(_unitOfWorkMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new ContributorProfileValidator(
                null
            ));
        }

        [Fact]
        public void TestProfileValidator_Success()
        {
            // Arrange

            // Act
            var result = new ContributorProfileValidator(_unitOfWorkMock.Object).Validate(_contributorProfile);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void TestProfileValidator_Invalid()
        {
            // Arrange
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Find(It.IsAny<Expression<Func<ArcadeMode,bool>>>())).Returns(new List<ArcadeMode>());
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefault(y => y.Key == ConfigKeys.OW_MAPS.ToString())).Returns((Config) null);
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefault(y => y.Key == ConfigKeys.OW_HEROES.ToString())).Returns((Config) null);
            
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