using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.API.Services.ConfigService;
using OverwatchArcade.API.Services.OverwatchService;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence;
using OverwatchArcade.Persistence.Repositories.Interfaces;
using OverwatchArcade.Twitter.Dtos;
using OverwatchArcade.Twitter.Services.TwitterService;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Services
{
    public class OverwatchServiceTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<IConfigService> _configServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ITwitterService> _twitterServiceMock;
        private readonly Mock<ILogger<OverwatchService>> _loggerMock;
        private readonly Mock<IValidator<CreateDailyDto>> _validatorMock;
        private readonly Mock<IContributorRepository> _contributorRepositoryMock;

        private readonly OverwatchService _overwatchService;
        private Contributor _contributor;
        private Daily _daily;
        private CreateDailyDto _createDailyDto;

        public OverwatchServiceTest()
        {
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _configServiceMock = new Mock<IConfigService>();
            _configurationMock = new Mock<IConfiguration>();
            _twitterServiceMock = new Mock<ITwitterService>();
            _loggerMock = new Mock<ILogger<OverwatchService>>();
            _validatorMock = new Mock<IValidator<CreateDailyDto>>();
            _contributorRepositoryMock = new Mock<IContributorRepository>();

            _overwatchService = new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object, _validatorMock.Object,
                _contributorRepositoryMock.Object);
            ConfigureMocks();
        }

        private void ConfigureMocks()
        {
            _contributor = new Contributor()
            {
                Id = new Guid("12D20088-719F-48A3-859D-A255CDFD1273"),
                Email = "info@overwatcharcade.today",
                Username = "System",
                Group = ContributorGroup.Developer,
                Avatar = "default.jpg"
            };
            _createDailyDto = new CreateDailyDto()
            {
                TileModes = new List<CreateTileModeDto>()
                {
                    new()
                    {
                        TileId = 1,
                        ArcadeModeId = 1,
                        LabelId = 1,
                    },
                    new()
                    {
                        TileId = 2,
                        ArcadeModeId = 2,
                        LabelId = 1,
                    },
                    new()
                    {
                        TileId = 3,
                        ArcadeModeId = 3,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 4,
                        ArcadeModeId = 4,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 5,
                        ArcadeModeId = 5,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 6,
                        ArcadeModeId = 6,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 7,
                        ArcadeModeId = 7,
                        LabelId = 2,
                    }
                }
            };
            
            _daily = new Daily()
            {
                CreatedAt = DateTime.UtcNow,
                MarkedOverwrite = false,
                TileModes = new List<TileMode>()
                {
                    new()
                    {
                        TileId = 1,
                        ArcadeModeId = 1,
                        DailyId = 1,
                        LabelId = 1,
                    },
                    new()
                    {
                        TileId = 2,
                        ArcadeModeId = 2,
                        DailyId = 1,
                        LabelId = 1,
                    },
                    new()
                    {
                        TileId = 3,
                        ArcadeModeId = 3,
                        DailyId = 1,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 4,
                        ArcadeModeId = 4,
                        DailyId = 1,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 5,
                        ArcadeModeId = 5,
                        DailyId = 1,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 6,
                        ArcadeModeId = 6,
                        DailyId = 1,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 7,
                        ArcadeModeId = 7,
                        DailyId = 1,
                        LabelId = 2,
                    }
                }
            };
        }

        [Fact]
        public void TestConstructor()
        {
            var constructor = new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object, _validatorMock.Object,
                _contributorRepositoryMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new OverwatchService(null, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object,
                _validatorMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, null, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object,
                _validatorMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, null, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object,
                _validatorMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, null, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object,
                _validatorMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, null, _twitterServiceMock.Object, _loggerMock.Object,
                _validatorMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, null, _loggerMock.Object,
                _validatorMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, null,
                _validatorMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object,
                null, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object,
                _validatorMock.Object, null));
        }


        [Fact]
        public async Task TestSubmit_DailyAlreadySubmitted()
        {
            // arrange
            var owTilesConfigKey = new Config()
            {
                Value = "7"
            };
            var validateResultMock = new Mock<ValidationResult>();
            validateResultMock.Setup(x => x.IsValid).Returns(true);
            
            _unitOfWorkMock.Setup(x => x.ConfigRepository.Find(y => y.Key == "OW_TILES")).Returns(new List<Config> { owTilesConfigKey });
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Exists(It.IsAny<Expression<Func<ArcadeMode, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.LabelRepository.Exists(It.IsAny<Expression<Func<Label, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(true);
            _validatorMock.Setup(x => x.ValidateAsync(_createDailyDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);

            // act
            var result = await _overwatchService.Submit(_createDailyDto, _contributor.Id);

            // assert
            result.StatusCode.ShouldBe(409);
        }

        [Fact]
        public async Task TestSubmit_Throws_Exception()
        {
            var owTilesConfigKey = new Config()
            {
                Value = "7"
            };
            var validateResultMock = new Mock<ValidationResult>();
            validateResultMock.Setup(x => x.IsValid).Returns(true);
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.ConfigRepository.Find(y => y.Key == "OW_TILES")).Returns(new List<Config> { owTilesConfigKey });
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Exists(It.IsAny<Expression<Func<ArcadeMode, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.LabelRepository.Exists(It.IsAny<Expression<Func<Label, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetDaily()).Returns(_daily);
            _unitOfWorkMock.Setup(x => x.Save()).Throws<DbUpdateException>();
            _configurationMock.Setup(x => x.GetSection("connectToTwitter")).Returns(connectToTwitterConfigurationSection.Object);
            _validatorMock.Setup(x => x.ValidateAsync(_createDailyDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);

            // act
            var result = await _overwatchService
                .Submit(_createDailyDto, _contributor.Id);

            // assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(500);
        }

        [Fact(Skip = "Todo")]
        public async Task TestSubmit_SubmitsDaily()
        {
            var owTilesConfigKey = new Config()
            {
                Value = "7"
            };
            var createTweetDto = new CreateTweetDto()
            {
                CurrentEvent = "default",
                ScreenshotUrl = "https://overwatch.local/overwatch"
            };
            var expectedDailyDto = new DailyDto()
            {
                Contributor = new ContributorDto()
                {
                    Username = "System"
                }
            };
            var validateResultMock = new Mock<ValidationResult>();
            validateResultMock.Setup(x => x.IsValid).Returns(true);
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.ConfigRepository.Find(y => y.Key == "OW_TILES")).Returns(new List<Config> { owTilesConfigKey });
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Exists(It.IsAny<Expression<Func<ArcadeMode, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.LabelRepository.Exists(It.IsAny<Expression<Func<Label, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetDaily()).Returns(_daily);
            _twitterServiceMock.Setup(x => x.PostTweet(createTweetDto));
            _configurationMock.Setup(x => x.GetSection("connectToTwitter")).Returns(connectToTwitterConfigurationSection.Object);
            _mapperMock.Setup(x => x.Map<Daily>(_createDailyDto)).Returns(_daily);
            _validatorMock.Setup(x => x.ValidateAsync(_createDailyDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);

            // act
            var result = await _overwatchService.Submit(_createDailyDto, _contributor.Id);

            // assert
            _unitOfWorkMock.Verify(x => x.DailyRepository.Add(_daily));
            _unitOfWorkMock.Verify(x => x.Save());
            result.StatusCode.ShouldBe(200);
            result.Data.ShouldBeOfType<DailyDto>();
            result.Data.ShouldBeEquivalentTo(expectedDailyDto);
        }

        [Fact]
        public async Task TestUndo_NotSubmittedYet()
        {
            // arrange
            var contributorId = new Guid("9725B478-B92E-4453-B10D-D7DA61A1F6E8");
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(false);

            // act
            var result = await _overwatchService.Undo(contributorId, true);

            // assert
            result.StatusCode.ShouldBe(500);
        }

        [Fact]
        public async Task TestUndo_Undo_Delete()
        {
            // arrange
            var contributorId = new Guid("9725B478-B92E-4453-B10D-D7DA61A1F6E8");
            var daily = new Daily();
            var dailySubmits = new List<Daily>() { daily };
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.Find(It.IsAny<Expression<Func<Daily, bool>>>())).Returns(dailySubmits);
            _configurationMock.Setup(x => x.GetSection("connectToTwitter")).Returns(connectToTwitterConfigurationSection.Object);

            // act
            var result = await _overwatchService.Undo(contributorId, true);

            // assert
            result.StatusCode.ShouldBe(200);
            _unitOfWorkMock.Verify(x => x.DailyRepository.RemoveRange(dailySubmits));
            _unitOfWorkMock.Verify(x => x.Save());
        }

        [Fact]
        public async Task TestUndo_Undo_SoftDelete()
        {
            // arrange
            var contributorId = new Guid("9725B478-B92E-4453-B10D-D7DA61A1F6E8");
            var daily = new Daily();
            var dailySubmits = new List<Daily>() { daily };
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.Find(It.IsAny<Expression<Func<Daily, bool>>>())).Returns(dailySubmits);
            _configurationMock.Setup(x => x.GetSection("connectToTwitter")).Returns(connectToTwitterConfigurationSection.Object);

            // act
            var result = await _overwatchService.Undo(contributorId, false);

            // assert
            result.StatusCode.ShouldBe(200);
            _unitOfWorkMock.Verify(x => x.DailyRepository.RemoveRange(dailySubmits), Times.Never);
            dailySubmits.ShouldAllBe(x => x.MarkedOverwrite.Equals(true));
            _unitOfWorkMock.Verify(x => x.Save());
        }

        [Fact]
        public async Task TestUndo_Throws_Exception()
        {
            // arrange
            var contributorId = new Guid("9725B478-B92E-4453-B10D-D7DA61A1F6E8");
            var daily = new Daily();
            var dailySubmits = new List<Daily>() { daily };
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.Find(It.IsAny<Expression<Func<Daily, bool>>>())).Returns(dailySubmits);
            _unitOfWorkMock.Setup(x => x.Save()).Throws<DbUpdateException>();
            _configurationMock.Setup(x => x.GetSection("connectToTwitter")).Returns(connectToTwitterConfigurationSection.Object);


            // act
            var result = await _overwatchService.Undo(contributorId, false);

            // assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(500);
        }

        [Fact]
        public void TestGetDaily_ReturnsDaily()
        {
            // arrange
            var dailyDto = new DailyDto()
            {
                CreatedAt = DateTime.UtcNow,
                IsToday = true,
                Modes = new List<TileModeDto>()
                {
                    new()
                    {
                        Name = "Total Mayhem",
                        Description = "Everybody likes mayhem, right?",
                        Players = "6v6",
                        Image = "image.jpg",
                        Label = "Daily"
                    }
                },
                Contributor = new ContributorDto()
                {
                    Username = "System",
                    Avatar = "default.jpg",
                    Stats = new ContributorStats()
                    {
                        FavouriteContributionDay = "Saturday",
                        LastContributedAt = DateTime.Parse("03-21-2021")
                    }
                }
            };
            var expectedDailyDto = JsonConvert.DeserializeObject<DailyDto>(JsonConvert.SerializeObject(dailyDto)); // Ez deep clone

            _mapperMock.Setup(map => map.Map<DailyDto>(It.IsAny<object>())).Returns(dailyDto);
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetDaily()).Returns(_daily);

            // act
            var result = _overwatchService.GetDaily();

            // assert
            result.StatusCode.ShouldBe(200);
            result.Data.ShouldBeEquivalentTo(expectedDailyDto);
        }

        [Fact]
        public void TestGetArcadeModes_ReturnsListOfArcadeModes()
        {
            // arrange
            var arcadeModeList = new List<ArcadeMode>()
            {
                new()
                {
                    Image = "image.jpg",
                    Name = "Total Mayhem",
                    Description = "Everybody loves total mayhem!",
                    Players = "6v6"
                },
                new()
                {
                    Image = "image.jpg",
                    Name = "CTF",
                    Description = "Capture the flag!",
                    Players = "6v6"
                }
            };
            var arcadeModeDtoList = new List<ArcadeModeDto>()
            {
                new()
                {
                    Image = "image.jpg",
                    Name = "Total Mayhem",
                    Description = "Everybody loves total mayhem!",
                    Players = "6v6"
                },
                new()
                {
                    Image = "image.jpg",
                    Name = "Total Mayhem",
                    Description = "Everybody loves total mayhem!",
                    Players = "6v6"
                }
            };

            _mapperMock.Setup(x => x.Map<List<ArcadeModeDto>>(arcadeModeList)).Returns(arcadeModeDtoList);
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.GetArcadeModes()).Returns(arcadeModeList);

            var expectedArcadeModeList = JsonConvert.DeserializeObject<List<ArcadeModeDto>>(JsonConvert.SerializeObject(arcadeModeDtoList)); // Ez deep clone

            // act
            var result = _overwatchService.GetArcadeModes();

            // assert
            result.StatusCode.ShouldBe(200);
            result.Data.ShouldBeEquivalentTo(expectedArcadeModeList);
        }

        [Fact]
        public void TestGetLabels_ReturnsListOfLabels()
        {
            // arrange
            var labels = new List<Label>()
            {
                new()
                {
                    Value = null
                },
                new()
                {
                    Value = "Daily"
                },
                new()
                {
                    Value = "Weekly"
                },
                new()
                {
                    Value = "Permanent"
                }
            };
            var expectedLabels = JsonConvert.DeserializeObject<List<Label>>(JsonConvert.SerializeObject(labels));
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.GetLabels()).Returns(labels);

            // act
            var result = _overwatchService.GetLabels();

            // assert
            result.StatusCode.ShouldBe(200);
            result.Data.ShouldBeEquivalentTo(expectedLabels);
        }
    }
}