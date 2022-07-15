using System;
using System.Collections.Generic;
using System.Linq;
using OverwatchArcade.Domain.Factories;
using OverwatchArcade.Domain.Models.Overwatch;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Factories;

public class DailyFactoryTest
{
    private readonly DailyFactory _dailyFactory;
    public DailyFactoryTest()
    {
        _dailyFactory = new DailyFactory();
    }

    [Fact]
    public void Create()
    {
        // Arrange
        var userId = new Guid("BCC06CCF-BDB2-4D76-9E65-0928844AEE2A");
        var tileModes = new List<TileMode>()
        {
            new()
            {
                ArcadeModeId = 1337,
            }
        };

        // Act
        var daily = _dailyFactory.Create(userId, tileModes);
       
        // Assert
        daily.ShouldBeOfType<Daily>();
        daily.ContributorId.ShouldBe(userId);
        Assert.Single(daily.TileModes);
        daily.TileModes.First().ArcadeModeId.ShouldBe(1337);
    }
}