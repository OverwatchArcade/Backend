using Microsoft.AspNetCore.Mvc;
using OverwatchArcade.Twitter.Dtos;
using OverwatchArcade.Twitter.Services.ScreenshotService;
using OverwatchArcade.Twitter.Services.TwitterService;

namespace OverwatchArcade.Twitter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly ITwitterService _twitterService;

        public IndexController(ITwitterService twitterService, IScreenshotService screenshotService)
        {
            _twitterService = twitterService ?? throw new ArgumentNullException(nameof(twitterService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTweet(CreateTweetDto createTweetDto)
        {
            try
            {
                await _twitterService.PostTweet(createTweetDto);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTweet()
        {
            try
            {
                await _twitterService.DeleteLastTweet();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
