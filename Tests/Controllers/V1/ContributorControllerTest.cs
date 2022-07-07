using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        private Guid _userId;
        private ClaimsPrincipal _claimsPrincipalUser;
        private ContributorController _contributorController;

        public ContributorControllerTest()
        {
            _contributorServiceMock = new Mock<IContributorService>();

            ConstructTestObjects();
        }

        private void ConstructTestObjects()
        {
            _contributorDto = new ContributorDto
            {
                Username = "system",
                Avatar = "avatar.jpg",
                RegisteredAt = DateTime.Parse("01-01-2000"),
                Profile = new ContributorProfile
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
                    },
                    Personal = new About
                    {
                        Text = "I like writing Unit Tests",
                        Country = new Country
                        {
                            Name = "Netherlands",
                            Code = "NL"
                        }
                    },
                    Social = new Socials
                    {
                        Battlenet = "battlenet",
                        Discord = "Discord",
                        Steam = "steam",
                        Twitter = "Twitter"
                    }
                }
            };

            _userId = new Guid("78B994EC-9AD4-41B6-B059-761C1887DE0F");
            _claimsPrincipalUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId.ToString()),
            }));
            _contributorController = new ContributorController(_contributorServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = _claimsPrincipalUser
                    }
                }
            };
        }

        [Fact]
        public void Constructor()
        {
            ContributorController constructor = new ContributorController(_contributorServiceMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void ConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new ContributorController(
                null
            ));
        }


        [Fact]
        public async Task GetAllContributors_Returns_ListOfContributors()
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
            var result = await _contributorController.GetAllContributors();

            // Assert
            result.ShouldBeOfType<ObjectResult>();
            var returnedValue = result as ObjectResult;
            returnedValue.ShouldNotBeNull();
            returnedValue.Value.ShouldBeEquivalentTo(expectedResponse);
        }

        [Fact]
        public void GetContributorByUsername_Returns_Contributor()
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
            var result = _contributorController.GetContributorByUsername(username);

            // Assert
            result.ShouldBeOfType<ObjectResult>();
            var returnedValue = result as ObjectResult;
            returnedValue.ShouldNotBeNull();
            returnedValue.Value.ShouldBeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task SaveProfile()
        {
            // Arrange
            var contributorProfile = new ContributorProfileDto()
            {
                Personal = new About()
                {
                    Text = "I love unit tests!"
                }
            };
            var expectedResponse = new ServiceResponse<ContributorDto>()
            {
                Data = new ContributorDto()
                {
                    Profile = new ContributorProfile()
                    {
                        Personal = new About()
                        {
                            Text = "I love unit tests!"
                        }
                    }
                }
            };

            _contributorServiceMock.Setup(cs => cs.SaveProfile(contributorProfile, _userId)).ReturnsAsync(expectedResponse);

            // Act
            var actionResult = await _contributorController.SaveProfile(contributorProfile);

            // Assert
            actionResult.ShouldBeOfType<ObjectResult>();
            var result = actionResult as ObjectResult;
            var serviceResponseResult = (ServiceResponse<ContributorDto>)result?.Value;
            serviceResponseResult?.ShouldBeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task SaveAvatar()
        {
            // Arrange
            var avatarMock = new Mock<IFormFile>();

            var contributorAvatar = new ContributorAvatarDto()
            {
                Avatar = avatarMock.Object
            };
            var expectedResponse = new ServiceResponse<ContributorDto>();

            _contributorServiceMock.Setup(cs => cs.SaveAvatar(contributorAvatar, _userId)).ReturnsAsync(expectedResponse);

            // Act
            var actionResult = await _contributorController.SaveAvatar(contributorAvatar);

            // Assert
            _contributorServiceMock.Verify(cs => cs.SaveAvatar(contributorAvatar, _userId));
            actionResult.ShouldBeOfType<ObjectResult>();
            var result = actionResult as ObjectResult;
            var serviceResponseResult = (ServiceResponse<ContributorDto>)result?.Value;
            serviceResponseResult?.ShouldBeEquivalentTo(expectedResponse);
        }
    }
}