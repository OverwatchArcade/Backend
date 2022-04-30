using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OWArcadeBackend.Controllers.V1;
using OWArcadeBackend.Dtos.Contributor;
using OWArcadeBackend.Dtos.Contributor.Profile;
using OWArcadeBackend.Dtos.Contributor.Profile.Personal;
using OWArcadeBackend.Models;
using OWArcadeBackend.Services.ContributorService;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Controllers.V1
{
    public class ContributorControllerTest
    {
        private readonly Mock<IContributorService> _contributorServiceMock;

        private ContributorDto _contributorDto;

        public ContributorControllerTest()
        {
            _contributorServiceMock = new Mock<IContributorService>();
            
            ConstructTestObjects();
        }

        private void ConstructTestObjects()
        {
            _contributorDto = new ContributorDto()
            {
                Username = "system",
                Avatar = "avatar.jpg",
                RegisteredAt = DateTime.Parse("01-01-2000"),
                Profile = new ContributorProfileDto()
                {
                    Game = new ()
                    {
                        Overwatch = new ()
                        {
                            ArcadeModes = new ()
                            {
                                new()
                                {
                                    Name = "Total Mayhem",
                                    Image = "image.jpg"
                                }
                            },
                            Maps = new ()
                            {
                                new()
                                {
                                    Name = "Ayutaha",
                                    Image = "image.jpg",
                                }
                            },
                            Heroes = new ()
                            {
                                new()
                                {
                                    Name = "Soldier-76",
                                    Image = "Soldier.jpg"
                                }
                            }
                        }
                    },
                    Personal = new AboutDto()
                    {
                        About = "I like writing Unit Tests",
                        Country = new Country()
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
            var date = DateTime.Parse("03-20-2021");
            var serviceResponse = new ServiceResponse<List<ContributorDto>>
            {
                Data = new List<ContributorDto>()
                {
                    _contributorDto
                },
                Time = date
            };
            var expectedResponse = new ServiceResponse<List<ContributorDto>>
            {
                Data = new List<ContributorDto>()
                {
                    _contributorDto
                },
                Time = date
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
        public async Task TestGetContributorByUsername_Returns_Contributor()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            const string username = "system";

            var serviceResponse = new ServiceResponse<ContributorDto>
            {
                Data = _contributorDto,
                Time = date
            };
            var expectedResponse = new ServiceResponse<ContributorDto>
            {
                Data = _contributorDto,
                Time = date
            };
            _contributorServiceMock.Setup(x => x.GetContributorByUsername(username)).ReturnsAsync(serviceResponse);

            // Act
            var result = await new ContributorController(_contributorServiceMock.Object).GetContributorByUsername(username);

            // Assert
            result.ShouldBeOfType<ObjectResult>();
            var returnedValue = result as ObjectResult;
            returnedValue.ShouldNotBeNull();
            returnedValue.Value.ShouldBeEquivalentTo(expectedResponse);
        }
    }
}