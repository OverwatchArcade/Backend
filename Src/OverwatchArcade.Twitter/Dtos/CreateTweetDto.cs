namespace OverwatchArcade.Twitter.Dtos;

public class CreateTweetDto
{
    public string ScreenshotUrl { get; set; }
    public string CurrentEvent { get; set; } = "default";
}