using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OWArcadeBackend.Dtos.Contributor;
using OWArcadeBackend.Dtos.Contributor.Profile;
using OWArcadeBackend.Dtos.Contributor.Profile.Game.Overwatch.Portraits;
using OWArcadeBackend.Dtos.Contributor.Profile.Personal;
using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence;
using OWArcadeBackend.Services.ContributorService;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Services
{
    public class ContributorServiceTest
    {
        private Mock<ILogger<ContributorService>> _loggerMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;

        public ContributorServiceTest()
        {
            _loggerMock = new Mock<ILogger<ContributorService>>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        [Fact]
        public void TestConstructor()
        {
            var constructor = new ContributorService(_loggerMock.Object, _mapperMock.Object, _unitOfWorkMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new ContributorService(
                null,
                _mapperMock.Object,
                _unitOfWorkMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new ContributorService(
                _loggerMock.Object,
                null,
                _unitOfWorkMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new ContributorService(
                _loggerMock.Object,
                _mapperMock.Object,
                null
            ));
        }

        [Fact]
        public async Task TestGetAllContributors_Returns_ListOfContributors()
        {
            // arrange
            var contributors = new List<Contributor>
            {
                new()
                {
                    Id = new Guid("62A1EA25-BD54-47F9-BACE-B03A1B8AEC95"),
                    Avatar = "image.jpg",
                    Email = "system@overwatcharcade.today",
                    Username = "System",
                    RegisteredAt = DateTime.Parse("03-20-2000"),
                    Profile = new ContributorProfileDto()
                    {
                        Game = new ()
                        {
                            Overwatch = new()
                            {
                                ArcadeModes = new ()
                                {
                                    new()
                                    {
                                        Image = "image.jpg",
                                        Name = "Total Mayhem",
                                    }
                                },
                                Maps = new List<Map>
                                {
                                    new()
                                    {
                                        Image = "image.jpg",
                                        Name = "Ayutthaya"
                                    }
                                },
                                Heroes = new List<Hero>
                                {
                                    new()
                                    {
                                        Image = "image.jpg",
                                        Name = "Soldier-76"
                                    }
                                }
                            }
                        },
                        Personal = new AboutDto()
                        {
                            About = "I love writing unit tests",
                            Country = new Country()
                            {
                                Name = "Netherlands",
                                Code = "NL"
                            }
                        },
                        Social = new SocialsDto()
                        {
                            Battlenet = "Battlenet#1234",
                            Steam = "overwatcharcade",
                            Twitter = "owarcade",
                            Discord = "Discord#1234"
                        }
                    }
                }
            };
            var contributorDtos = new List<ContributorDto>()
            {
                new()
                {
                    Avatar = "image.jpg",
                    Username = "System",
                    RegisteredAt = DateTime.Parse("03-20-2000"),
                }
            };
            var expectedContributorDtos = new List<ContributorDto>(contributorDtos);

            _unitOfWorkMock.Setup(x => x.ContributorRepository.GetAll()).ReturnsAsync(contributors);
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetContributedCount(contributors[0].Id)).ReturnsAsync(1);
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetLastContribution(contributors[0].Id)).ReturnsAsync(DateTime.Parse("03-20-2000"));
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetFavouriteContributionDay(contributors[0].Id)).Returns("Saturday");
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetContributionDays(contributors[0].Id)).Returns(new List<DateTime>
            {
                new(2021, 03, 20)
            });
            
            _mapperMock.Setup(x => x.Map<List<ContributorDto>>(contributors))
                .Returns(contributorDtos);

            // act
            var result = await new ContributorService(_loggerMock.Object, _mapperMock.Object, _unitOfWorkMock.Object).GetAllContributors();

            // assert
            result.Data.ShouldBeEquivalentTo(expectedContributorDtos);
        }

        [Fact]
        public async Task TestGetContributorByUsername_Returns_Contributor()
        {
            // arrange
            var contributor = new Contributor()
            {
                Id = new Guid("62A1EA25-BD54-47F9-BACE-B03A1B8AEC95"),
                Avatar = "image.jpg",
                Email = "system@overwatcharcade.today",
                Username = "System",
            };
            var contributorDto = new ContributorDto()
            {
                Avatar = "image.jpg",
                Username = "System",
            };
            var expectedContributorDtos = new ContributorDto()
            {
                Avatar = "image.jpg",
                Username = "System",
            };
            
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetContributedCount(contributor.Id)).ReturnsAsync(1);
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetLastContribution(contributor.Id)).ReturnsAsync(DateTime.Parse("03-20-2000"));
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetFavouriteContributionDay(contributor.Id)).Returns("Saturday");
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Find(It.IsAny<Expression<Func<Contributor,bool>>>())).Returns(new List<Contributor> { contributor });
            _mapperMock.Setup(x => x.Map<ContributorDto>(It.IsAny<Contributor>()))
                .Returns(contributorDto);
            
            // act
            var result = await new ContributorService(_loggerMock.Object, _mapperMock.Object, _unitOfWorkMock.Object).GetContributorByUsername(contributor.Username);
            
            // assert
            result.Data.ShouldBeEquivalentTo(expectedContributorDtos);
        }

        [Fact]
        public async Task TestGetContributorByUsername_Returns_404ServiceResponse()
        {
            // arrange
            const string contributorName = "System";
            
            // act
            var result = await new ContributorService(_loggerMock.Object, _mapperMock.Object, _unitOfWorkMock.Object).GetContributorByUsername(contributorName);

            // assert
            result.Data.ShouldBeNull();
            result.Message.ShouldBe($"Contributor {contributorName} not found");
            result.StatusCode.ShouldBe(404);
        }
    }
}