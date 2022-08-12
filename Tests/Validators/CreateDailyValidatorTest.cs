using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.API.Validators.Overwatch;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Validators
{
    public class CreateDailyValidatorTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private CreateDailyDto _createDailyDto;

        public CreateDailyValidatorTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            PrepareMock();
        }

        private void PrepareMock()
        {
            _createDailyDto = new CreateDailyDto()
            {
                TileModes = new List<CreateTileModeDto>
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
                        ArcadeModeId = 1,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 3,
                        ArcadeModeId = 1,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 4,
                        ArcadeModeId = 1,
                        LabelId = 1,
                    },
                    new()
                    {
                        TileId = 5,
                        ArcadeModeId = 1,
                        LabelId = 1,
                    },
                    new()
                    {
                        TileId = 6,
                        ArcadeModeId = 1,
                        LabelId = 1,
                    },
                    new()
                    {
                        TileId = 7,
                        ArcadeModeId = 1,
                        LabelId = 2,
                    },
                }
            };

            var databaseResultConfig = new Config()
            {
                Id = 1,
                Value = "7"
            };

            _unitOfWorkMock
                .Setup(x => x.ConfigRepository.FirstOrDefaultASync(It.IsAny<Expression<Func<Config, bool>>>()))
                .ReturnsAsync(databaseResultConfig);
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Exists(It.IsAny<Expression<Func<ArcadeMode, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.LabelRepository.Exists(It.IsAny<Expression<Func<Label, bool>>>())).Returns(true);
        }
        
        
        [Fact]
        public void Constructor()
        {
            var constructor = new CreateDailyDtoValidator(_unitOfWorkMock.Object);
            Assert.NotNull(constructor);
        }
        
        [Fact]
        public void ConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new CreateDailyDtoValidator(
                null
            ));
        }

        [Fact]
        public async Task DailyValidator_Success()
        {
            // Arrange
            
            // Act
            var validator = new CreateDailyDtoValidator(_unitOfWorkMock.Object);
            var result = await validator.ValidateAsync(_createDailyDto);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task DailyValidator_Invalid_TooManyTiles()
        {
            // Arrange
            _createDailyDto.TileModes.Add(new CreateTileModeDto { TileId = 8});
            
            // Act
            var validator = new CreateDailyDtoValidator(_unitOfWorkMock.Object);
            var result = await validator.ValidateAsync(_createDailyDto);
            
            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("Must have exactly the configured amount of tiles. I either received too much/little or received duplicate TileIds.", result.Errors[0].ErrorMessage);
        }
    }
}