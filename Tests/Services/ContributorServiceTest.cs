using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Factories.Interfaces;
using OverwatchArcade.API.Services.ContributorService;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Domain.Models.ContributorInformation.Game;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Models.ContributorInformation.Personal;
using OverwatchArcade.Persistence;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Services
{
    public class ContributorServiceTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<ContributorService>> _loggerMock;
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private readonly Mock<IValidator<ContributorAvatarDto>> _contributorAvatarValidatorMock;
        private readonly Mock<IValidator<ContributorProfileDto>> _contributorProfileValidatorMock;
        private readonly Mock<IServiceResponseFactory<ContributorDto>> _serviceResponseFactoryMock;

        private readonly ContributorService _contributorService;

        public ContributorServiceTest()
        {
            _loggerMock = new Mock<ILogger<ContributorService>>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _contributorAvatarValidatorMock = new Mock<IValidator<ContributorAvatarDto>>();
            _contributorProfileValidatorMock = new Mock<IValidator<ContributorProfileDto>>();
            _serviceResponseFactoryMock = new Mock<IServiceResponseFactory<ContributorDto>>();
            
            _contributorService = new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, _loggerMock.Object, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object, _serviceResponseFactoryMock.Object);
        }

        [Fact]
        public void TestConstructor()
        {
            var constructor = new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, _loggerMock.Object, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object, _serviceResponseFactoryMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new ContributorService(null, _unitOfWorkMock.Object, _loggerMock.Object, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object, _serviceResponseFactoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new ContributorService(_mapperMock.Object, null, _loggerMock.Object, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object, _serviceResponseFactoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, null, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object, _serviceResponseFactoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, _loggerMock.Object, null, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object, _serviceResponseFactoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, _loggerMock.Object, _webHostEnvironmentMock.Object, null, _contributorProfileValidatorMock.Object, _serviceResponseFactoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, _loggerMock.Object, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, null, _serviceResponseFactoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, _loggerMock.Object, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object, null));
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
                        Game = new Games
                        {
                            Overwatch = new OverwatchProfile
                            {
                                ArcadeModes = new List<ArcadeModePortrait>
                                {
                                    new()
                                    {
                                        Image = "image.jpg",
                                        Name = "Total Mayhem",
                                    }
                                },
                                Maps = new List<MapPortrait>
                                {
                                    new()
                                    {
                                        Image = "image.jpg",
                                        Name = "Ayutthaya"
                                    }
                                },
                                Heroes = new List<HeroPortrait>
                                {
                                    new()
                                    {
                                        Image = "image.jpg",
                                        Name = "Soldier-76"
                                    }
                                }
                            }
                        },
                        Personal = new About
                        {
                            Text = "I love writing unit tests",
                            Country = new Country
                            {
                                Name = "Netherlands",
                                Code = "NL"
                            }
                        },
                        Social = new Socials
                        {
                            Battlenet = "Battlenet#1234",
                            Steam = "overwatcharcade",
                            Twitter = "owarcade",
                            Discord = "Discord#1234"
                        }
                    }
                }
            };
            var contributorDtos = new List<ContributorDto>
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
            var result = await _contributorService.GetAllContributors();

            // assert
            result.Data.ShouldBeEquivalentTo(expectedContributorDtos);
        }

        [Fact(Skip = "Todo")]
        public void TestGetContributorByUsername_Returns_Contributor()
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
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Find(It.IsAny<Expression<Func<Contributor, bool>>>())).Returns(new List<Contributor> { contributor });
            _mapperMock.Setup(x => x.Map<ContributorDto>(It.IsAny<Contributor>()))
                .Returns(contributorDto);

            // act
            var result = _contributorService.GetContributorByUsername(contributor.Username);

            // assert
            result.Data.ShouldBeEquivalentTo(expectedContributorDtos);
        }
    }
}