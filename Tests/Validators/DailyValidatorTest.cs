using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Persistence;
using OWArcadeBackend.Validators;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Validators
{
    public class DailyValidatorTest
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Daily _daily;

        public DailyValidatorTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            PrepareMock();
        }

        private void PrepareMock()
        {
            _daily = new Daily()
            {
                Id = 1,
                ContributorId = new Guid("1B8DE1B6-FD88-4623-B844-968B145505CF"),
                TileModes = new List<TileMode>()
                {
                    new()
                    {
                        TileId = 1,
                        ArcadeModeId = 1,
                        ArcadeMode = new ArcadeMode()
                        {
                            Game = Game.OVERWATCH,
                            Name = "Test",
                            Description = "Test"
                        }
                    },
                    new()
                    {
                        TileId = 2,
                        ArcadeModeId = 1,
                        ArcadeMode = new ArcadeMode()
                        {
                            Game = Game.OVERWATCH,
                            Name = "Test",
                            Description = "Test"
                        }
                    },
                    new()
                    {
                        TileId = 3,
                        ArcadeModeId = 1,
                        ArcadeMode = new ArcadeMode()
                        {
                            Game = Game.OVERWATCH,
                            Name = "Test",
                            Description = "Test"
                        }
                    },
                    new()
                    {
                        TileId = 4,
                        ArcadeModeId = 1,
                        ArcadeMode = new ArcadeMode()
                        {
                            Game = Game.OVERWATCH,
                            Name = "Test",
                            Description = "Test"
                        }
                    },
                    new()
                    {
                        TileId = 5,
                        ArcadeModeId = 1,
                        ArcadeMode = new ArcadeMode()
                        {
                            Game = Game.OVERWATCH,
                            Name = "Test",
                            Description = "Test"
                        }
                    },
                    new()
                    {
                        TileId = 6,
                        ArcadeModeId = 1,
                        ArcadeMode = new ArcadeMode()
                        {
                            Game = Game.OVERWATCH,
                            Name = "Test",
                            Description = "Test"
                        }
                    },
                    new()
                    {
                        TileId = 7,
                        ArcadeModeId = 1,
                        ArcadeMode = new ArcadeMode()
                        {
                            Game = Game.OVERWATCH,
                            Name = "Test",
                            Description = "Test"
                        }
                    },
                }
            };
            
            IEnumerable<Config> databaseResultConfig = new[]
            {
                new Config()
                {
                    Id = 1,
                    Value = "7"
                }
            };
            
            _unitOfWorkMock.Setup(x => x.ConfigRepository.Find(It.IsAny<Expression<Func<Config, bool>>>())).Returns(databaseResultConfig);
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Exists(It.IsAny<Expression<Func<ArcadeMode, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.LabelRepository.Exists(It.IsAny<Expression<Func<Label, bool>>>())).Returns(true);
        }
        
        
        [Fact]
        public void TestConstructor()
        {
            var constructor = new DailyValidator(_unitOfWorkMock.Object, Game.OVERWATCH);
            Assert.NotNull(constructor);
        }
        
        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new DailyValidator(
                null,
                Game.OVERWATCH
            ));
        }

        [Fact]
        public void TestDailyValidator_Success()
        {
            // Arrange
            
            // Act
            var result = new DailyValidator(_unitOfWorkMock.Object, Game.OVERWATCH).Validate(_daily);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void TestDailyValidator_Invalid_TooManyTiles()
        {
            // Arrange
            _daily.TileModes.Add(new TileMode { TileId = 8});
            
            // Act
            var result = new DailyValidator(_unitOfWorkMock.Object, Game.OVERWATCH).Validate(_daily);
            
            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("OVERWATCH Must have exactly 7 amount of tiles. I either received too much/little or received duplicate TileIds.", result.Errors[0].ErrorMessage);
        }
    }
}