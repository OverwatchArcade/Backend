using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OWArcadeBackend.Dtos;
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
                    Profile = new ContributorProfile
                    {
                        Game = new ContributorProfileGame
                        {
                            Overwatch = new ContributorProfileGameOverwatch
                            {
                                ArcadeModes = new List<ArcadeModeSettingDto>
                                {
                                    new()
                                    {
                                        Image = "image.jpg",
                                        Name = "Total Mayhem",
                                    }
                                },
                                Maps = new List<ConfigOverwatchMap>
                                {
                                    new()
                                    {
                                        Image = "image.jpg",
                                        Name = "Ayutthaya"
                                    }
                                },
                                Heroes = new List<ConfigOverwatchHero>
                                {
                                    new()
                                    {
                                        Image = "image.jpg",
                                        Name = "Soldier-76"
                                    }
                                }
                            }
                        },
                        Personal = new ContributorProfileAbout()
                        {
                            About = "I love writing unit tests",
                            Country = new ConfigCountries()
                            {
                                Name = "Netherlands",
                                Code = "NL"
                            }
                        },
                        Social = new ContributorProfileSocials()
                        {
                            Battlenet = "Battlenet#1234",
                            Steam = "overwatcharcade",
                            Twitter = "owarcade",
                            Discord = "Discord#1234"
                        }
                    }
                }
            };
            var expectedContributors = new List<Contributor>(contributors);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.GetAll()).ReturnsAsync(contributors);
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetContributedCount(contributors[0].Id)).ReturnsAsync(1);
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetLastContribution(contributors[0].Id)).ReturnsAsync(DateTime.Parse("03-20-2000"));
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetFavouriteContributionDay(contributors[0].Id)).Returns("Saturday");
            _mapperMock.Setup(x => x.Map<List<Contributor>>(It.IsAny<IEnumerable<Contributor>>())).Returns(contributors.ToList());
            
            // act
            var result = await new ContributorService(_loggerMock.Object, _mapperMock.Object, _unitOfWorkMock.Object).GetAllContributors();

            // assert
            result.Data.ShouldBeEquivalentTo(expectedContributors);
        }
    }
}