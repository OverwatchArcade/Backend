namespace OverwatchArcade.Twitter.Services.ScreenshotService;

public interface IScreenshotService
{
    public Task<byte[]> CreateScreenshot(string screenshotUrl);
}