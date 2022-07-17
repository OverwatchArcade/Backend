using System;
using System.Collections.Generic;
using System.Linq;
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
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.API.Services.ConfigService;
using OverwatchArcade.API.Services.ContributorService;
using OverwatchArcade.API.Services.OverwatchService;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence;
using OverwatchArcade.Persistence.Repositories.Interfaces;
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
        private readonly Mock<IContributorService> _contributorServiceMock;
        private readonly Mock<IContributorRepository> _contributorRepositoryMock;

        private readonly OverwatchService _overwatchService;
        private Contributor _contributor;
        private Daily _daily;
        private CreateDailyDto _createDailyDto;
        private DailyDto _dailyDto;
        private DailyDto _expectedDailyDto;
        private Config _owTilesConfigKey;
        
        private const string ConfigTilesKey = "OW_TILES";
        private const string ConfigTwitterKey = "connectToTwitter";
        private const string ConfigTwitterScreenshotUrl = "ScreenshotUrl";

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
            _contributorServiceMock = new Mock<IContributorService>();
            _contributorRepositoryMock = new Mock<IContributorRepository>();

            _overwatchService = new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object, _validatorMock.Object,
                _contributorServiceMock.Object, _contributorRepositoryMock.Object);
            ConfigureTestData();
        }

        private void ConfigureTestData()
        {
            _owTilesConfigKey = new Config
            {
                Value = "7"
            };
            
            _contributor = new Contributor
            {
                Id = new Guid("12D20088-719F-48A3-859D-A255CDFD1273"),
                Email = "info@overwatcharcade.today",
                Username = "System",
                Group = ContributorGroup.Developer,
                Avatar = "default.jpg"
            };

            _createDailyDto = new CreateDailyDto();

            _dailyDto = new DailyDto
            {
                CreatedAt = DateTime.UtcNow,
                IsToday = true,
                Modes = new List<TileModeDto>
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
                Contributor = new ContributorDto
                {
                    Username = "System",
                    Avatar = "default.jpg",
                    Stats = new ContributorStatsDto
                    {
                        FavouriteContributionDay = "Saturday",
                        LastContributedAt = DateTime.Parse("03-21-2021")
                    }
                }
            };
            
            _expectedDailyDto = JsonConvert.DeserializeObject<DailyDto>(JsonConvert.SerializeObject(_dailyDto)); // Ez deep clone
            
            _daily = new Daily
            {
                CreatedAt = DateTime.UtcNow,
                MarkedOverwrite = false,
            };
        }

        [Fact]
        public void Constructor()
        {
            var constructor = new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object, _validatorMock.Object,
                _contributorServiceMock.Object, _contributorRepositoryMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void ConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new OverwatchService(null, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object, _validatorMock.Object,
                _contributorServiceMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, null, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object, _validatorMock.Object,
                _contributorServiceMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, null, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object, _validatorMock.Object,
                _contributorServiceMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, null, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object, _validatorMock.Object,
                _contributorServiceMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, null, _twitterServiceMock.Object, _loggerMock.Object, _validatorMock.Object,
                _contributorServiceMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, null, _loggerMock.Object, _validatorMock.Object,
                _contributorServiceMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, null, _validatorMock.Object,
                _contributorServiceMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object, null,
                _contributorServiceMock.Object, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object, _validatorMock.Object,
                null, _contributorRepositoryMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_mapperMock.Object, _unitOfWorkMock.Object, _memoryCache, _configServiceMock.Object, _configurationMock.Object, _twitterServiceMock.Object, _loggerMock.Object, _validatorMock.Object,
                _contributorServiceMock.Object, null));
        }


        [Fact]
        public async Task Submit_DailyAlreadySubmitted()
        {
            // Arrange
            var validateResultMock = new Mock<ValidationResult>();
            validateResultMock.Setup(x => x.IsValid).Returns(true);
            
            _unitOfWorkMock.Setup(x => x.ConfigRepository.Find(y => y.Key == ConfigTilesKey)).Returns(new List<Config> { _owTilesConfigKey });
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Exists(It.IsAny<Expression<Func<ArcadeMode, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.LabelRepository.Exists(It.IsAny<Expression<Func<Label, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(true);
            _validatorMock.Setup(x => x.ValidateAsync(_createDailyDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);

            // Act
            var result = await _overwatchService.Submit(_createDailyDto, _contributor.Id);

            // Assert
            result.StatusCode.ShouldBe(409);
        }

        [Fact]
        public async Task Submit_Throws_Exception()
        {
            // Arrange
            var validateResultMock = new Mock<ValidationResult>();
            validateResultMock.Setup(x => x.IsValid).Returns(true);
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.ConfigRepository.Find(y => y.Key == ConfigTilesKey)).Returns(new List<Config> { _owTilesConfigKey });
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Exists(It.IsAny<Expression<Func<ArcadeMode, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.LabelRepository.Exists(It.IsAny<Expression<Func<Label, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetDaily()).Returns(_daily);
            _unitOfWorkMock.Setup(x => x.Save()).Throws<DbUpdateException>();
            _configurationMock.Setup(x => x.GetSection(ConfigTwitterKey)).Returns(connectToTwitterConfigurationSection.Object);
            _validatorMock.Setup(x => x.ValidateAsync(_createDailyDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);

            // Act
            var result = await _overwatchService
                .Submit(_createDailyDto, _contributor.Id);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(500);
        }
        
        [Fact]
        public async Task Submit_SubmitsDaily()
        {
            // Arrange
            var screenshotUrl = "https://overwatch.local/overwatch";
            var currentEvent = "default";

            var configServiceResponse = new ServiceResponse<string>
            {
                Data = "default"
            };
            
            var validateResultMock = new Mock<ValidationResult>();
            validateResultMock.Setup(x => x.IsValid).Returns(true);
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            var screenshotUrlConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            screenshotUrlConfigurationSection.Setup(x => x.Value).Returns("https://overwatch.local/overwatch");
            
            _unitOfWorkMock.Setup(x => x.ConfigRepository.Find(y => y.Key == ConfigTilesKey)).Returns(new List<Config> { _owTilesConfigKey });
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Exists(It.IsAny<Expression<Func<ArcadeMode, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.LabelRepository.Exists(It.IsAny<Expression<Func<Label, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetDaily()).Returns(_daily);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.GetContributedCount(_contributor.Id)).ReturnsAsync(1);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.GetLegacyContributionCount(_contributor.Id)).ReturnsAsync(5);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.GetLastContribution(_contributor.Id)).ReturnsAsync(DateTime.Now);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.GetContributionDays(_contributor.Id)).Returns(new List<DateTime>
            {
                DateTime.Now
            });
            _contributorRepositoryMock.Setup(x => x.FirstOrDefaultASync(It.IsAny<Expression<Func<Contributor, bool>>>())).ReturnsAsync(_contributor);
            _twitterServiceMock.Setup(x => x.PostTweet(screenshotUrl, currentEvent));
            _configServiceMock.Setup(x => x.GetCurrentOverwatchEvent()).ReturnsAsync(configServiceResponse);
            _configurationMock.Setup(x => x.GetSection(ConfigTwitterKey)).Returns(connectToTwitterConfigurationSection.Object);
            _configurationMock.Setup(x => x.GetSection(ConfigTwitterScreenshotUrl)).Returns(screenshotUrlConfigurationSection.Object);
            _mapperMock.Setup(x => x.Map<Daily>(_createDailyDto)).Returns(_daily);
            _mapperMock.Setup(x => x.Map<DailyDto>(_daily)).Returns(_dailyDto);
            _validatorMock.Setup(x => x.ValidateAsync(_createDailyDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);

            // Act
            var result = await _overwatchService.Submit(_createDailyDto, _contributor.Id);

            // Assert
            _unitOfWorkMock.Verify(x => x.DailyRepository.Add(_daily));
            _unitOfWorkMock.Verify(x => x.Save());
            result.StatusCode.ShouldBe(200);
            result.Data.ShouldBeOfType<DailyDto>();
            result.Data.ShouldBeEquivalentTo(_expectedDailyDto);
        }

        [Fact]
        public async Task Submit_Daily_Error_InvalidValidate()
        {
            // Arrange
            var validateResultMock = new Mock<ValidationResult>();
            validateResultMock.Setup(x => x.IsValid).Returns(false);
            _validatorMock.Setup(x => x.ValidateAsync(_createDailyDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);

            // Act
            var result = await _overwatchService.Submit(_createDailyDto, _contributor.Id);
            
            // Assert
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            Assert.Single(result.ErrorMessages);
        }

        [Fact]
        public async Task Undo_NotSubmittedYet()
        {
            // Arrange
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(false);

            // Act
            var result = await _overwatchService.Undo(_contributor.Id, true);

            // Assert
            result.StatusCode.ShouldBe(500);
        }

        [Fact]
        public async Task Undo_HardDelete()
        {
            // Arrange
            var dailySubmits = new List<Daily>() { _daily };
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.Find(It.IsAny<Expression<Func<Daily, bool>>>())).Returns(dailySubmits);
            _contributorRepositoryMock.Setup(x => x.FirstOrDefaultASync(It.IsAny<Expression<Func<Contributor, bool>>>())).ReturnsAsync(_contributor);
            _configurationMock.Setup(x => x.GetSection(ConfigTwitterKey)).Returns(connectToTwitterConfigurationSection.Object);

            // Act
            var result = await _overwatchService.Undo(_contributor.Id, true);

            // Assert
            result.StatusCode.ShouldBe(200);
            _unitOfWorkMock.Verify(x => x.DailyRepository.HardDeleteDaily());
        }

        [Fact]
        public async Task Undo_SoftDelete()
        {
            // Arrange
            var dailySubmits = new List<Daily> { _daily };
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.Find(It.IsAny<Expression<Func<Daily, bool>>>())).Returns(dailySubmits);
            _contributorRepositoryMock.Setup(x => x.FirstOrDefaultASync(It.IsAny<Expression<Func<Contributor, bool>>>())).ReturnsAsync(_contributor);
            _configurationMock.Setup(x => x.GetSection(ConfigTwitterKey)).Returns(connectToTwitterConfigurationSection.Object);

            // Act
            var result = await _overwatchService.Undo(_contributor.Id, false);

            // Assert
            result.StatusCode.ShouldBe(200);
            _unitOfWorkMock.Verify(x => x.DailyRepository.SoftDeleteDaily());
        }

        [Fact]
        public async Task Undo_AlreadyNoDailySubmitted_Returns_Error()
        {
            // Arrange
            _contributorRepositoryMock.Setup(x => x.FirstOrDefaultASync(It.IsAny<Expression<Func<Contributor, bool>>>())).ReturnsAsync(_contributor);
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(false);

            // Act
            var result = await _overwatchService.Undo(_contributor.Id, false);

            // Assert
            result.StatusCode.ShouldBe(500);
            Assert.Single(result.ErrorMessages);
            Assert.Equal("Daily has not been submitted yet", result.ErrorMessages.First());
        }

        [Fact]
        public async Task Undo_HardDelete_DeletesSubmittedTweet()
        {
            // Arrange
            var dailySubmits = new List<Daily> { _daily };
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("true");
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.Find(It.IsAny<Expression<Func<Daily, bool>>>())).Returns(dailySubmits);
            _contributorRepositoryMock.Setup(x => x.FirstOrDefaultASync(It.IsAny<Expression<Func<Contributor, bool>>>())).ReturnsAsync(_contributor);
            _configurationMock.Setup(x => x.GetSection(ConfigTwitterKey)).Returns(connectToTwitterConfigurationSection.Object);

            // Act
            var result = await _overwatchService.Undo(_contributor.Id, true);

            // Assert
            result.StatusCode.ShouldBe(200);
            _unitOfWorkMock.Verify(x => x.DailyRepository.HardDeleteDaily());
            _twitterServiceMock.Verify(x => x.DeleteLastTweet());
        }

        [Fact]
        public async Task Undo_DatabaseError_ThrowsException()
        {
            // Arrange
            var dailySubmits = new List<Daily>() { _daily };
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.Find(It.IsAny<Expression<Func<Daily, bool>>>())).Returns(dailySubmits);
            _unitOfWorkMock.Setup(x => x.Save()).Throws<DbUpdateException>();
            _configurationMock.Setup(x => x.GetSection(ConfigTwitterKey)).Returns(connectToTwitterConfigurationSection.Object);


            // Act
            var result = await _overwatchService.Undo(_contributor.Id, false);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(500);
        }

        [Fact]
        public void GetDaily_ReturnsDaily()
        {
            // Arrange
            _mapperMock.Setup(map => map.Map<DailyDto>(It.IsAny<object>())).Returns(_dailyDto);
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetDaily()).Returns(_daily);

            // Act
            var result = _overwatchService.GetDaily();

            // Assert
            result.StatusCode.ShouldBe(200);
            result.Data.ShouldBeEquivalentTo(_expectedDailyDto);
        }

        [Fact]
        public void GetArcadeModes_ReturnsListOfArcadeModes()
        {
            // Arrange
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

            // Act
            var result = _overwatchService.GetArcadeModes();

            // Assert
            result.StatusCode.ShouldBe(200);
            result.Data.ShouldBeEquivalentTo(expectedArcadeModeList);
        }

        [Fact]
        public void GetLabels_ReturnsListOfLabels()
        {
            // Arrange
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

            // Act
            var result = _overwatchService.GetLabels();

            // Assert
            result.StatusCode.ShouldBe(200);
            result.Data.ShouldBeEquivalentTo(expectedLabels);
        }
    }
}