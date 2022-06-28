using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OverwatchArcade.API.Controllers.V1;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Services.ContributorService;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Domain.Models.ContributorInformation.Game;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Models.ContributorInformation.Personal;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Controllers.V1
{
    public class ContributorControllerTest
    {
        private readonly Mock<IContributorService> _contributorServiceMock;

        private ContributorDto _contributorDto;
        private string _username;

        public ContributorControllerTest()
        {
            _contributorServiceMock = new Mock<IContributorService>();
            
            ConstructTestObjects();
        }

        private void ConstructTestObjects()
        {
            _username = "system";

            _contributorDto = new ContributorDto()
            {
                Username = "system",
                Avatar = "avatar.jpg",
                RegisteredAt = DateTime.Parse("01-01-2000"),
                Profile = new ContributorProfile()
                {
                    Game = new Games
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
                                    Name = "Ayutaha",
                                    Image = "image.jpg",
                                }
                            },
                            Heroes = new List<HeroPortrait>
                            {
                                new()
                                {
                                    Name = "Soldier-76",
                                    Image = "Soldier.jpg"
                                }
                            }
                        }
                    },
                    Personal = new About()
                    {
                        Text = "I like writing Unit Tests",
                        Country = new Country()
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
                }
            };
        }

        [Fact]
        public void TestConstructor()
        {
            ContributorController constructor = new ContributorController(_contributorServiceMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new ContributorController(
                null
            ));
        }
        

        [Fact]
        public async Task TestGetAllContributors_Returns_ListOfContributors()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<List<ContributorDto>>
            {
                Data = new List<ContributorDto>()
                {
                    _contributorDto
                }
            };
            var expectedResponse = new ServiceResponse<List<ContributorDto>>
            {
                Data = new List<ContributorDto>()
                {
                    _contributorDto
                }
            };
            _contributorServiceMock.Setup(x => x.GetAllContributors()).ReturnsAsync(serviceResponse);

            // Act
            var result = await new ContributorController(_contributorServiceMock.Object).GetAllContributors();

            // Assert
            result.ShouldBeOfType<ObjectResult>();
            var returnedValue = result as ObjectResult;
            returnedValue.ShouldNotBeNull();
            returnedValue.Value.ShouldBeEquivalentTo(expectedResponse);
        }

        [Fact]
        public void TestGetContributorByUsername_Returns_Contributor()
        {
            // Arrange
            const string username = "system";

            var serviceResponse = new ServiceResponse<ContributorDto>
            {
                Data = _contributorDto
            };
            var expectedResponse = new ServiceResponse<ContributorDto>
            {
                Data = _contributorDto
            };
            _contributorServiceMock.Setup(x => x.GetContributorByUsername(username)).Returns(serviceResponse);

            // Act
            var result = new ContributorController(_contributorServiceMock.Object).GetContributorByUsername(username);

            // Assert
            result.ShouldBeOfType<ObjectResult>();
            var returnedValue = result as ObjectResult;
            returnedValue.ShouldNotBeNull();
            returnedValue.Value.ShouldBeEquivalentTo(expectedResponse);
        }
    }
}