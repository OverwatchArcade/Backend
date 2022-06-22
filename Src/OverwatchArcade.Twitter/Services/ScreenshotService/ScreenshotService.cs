using Microsoft.Playwright;
using OverwatchArcade.Twitter.Dtos;

namespace OverwatchArcade.Twitter.Services.ScreenshotService;

public class ScreenshotService : IScreenshotService
{
    public async Task<byte[]> CreateScreenshot(CreateTweetDto createTweetDto)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();

        try
        {
            await page.GotoAsync(createTweetDto.ScreenshotUrl, new PageGotoOptions()
            {
                WaitUntil = WaitUntilState.NetworkIdle,
            });
            await page.WaitForTimeoutAsync(1000);
            var screenshot = await page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true, Type = ScreenshotType.Jpeg });
            await browser.CloseAsync();

            return screenshot;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}