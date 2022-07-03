using OverwatchArcade.Twitter.Dtos;

namespace OverwatchArcade.Twitter.Services.ScreenshotService;

public interface IScreenshotService
{
    public Task<byte[]> CreateScreenshot(CreateTweetDto createTweetDto);
}