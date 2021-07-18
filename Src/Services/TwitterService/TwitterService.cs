using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Twitter;
using OWArcadeBackend.Services.Twitter;

namespace OWArcadeBackend.Services.TwitterService
{
    public class TwitterService : ITwitterService
    {
        private readonly ILogger<TwitterService> _logger;
        private static readonly HttpClient Client = new();

        private readonly IOperations _operations;
        private IConfiguration _configuration;
        
        private const string URL = "https://api.apiflash.com/v1/urltoimage?";

        public TwitterService(ILogger<TwitterService> logger, IOperations operations, IConfiguration configuration)
        {
            _logger = logger;
            _operations = operations;
            _configuration = configuration;
        }

        private static Dictionary<string, string> CreateHttpParams(IConfiguration configuration)
        {
            return new()
            {
                {"access_key", configuration["APIFlash:Key"]},
                {"url", Environment.GetEnvironmentVariable("FRONTEND_URL") + "/overwatch" },
                {"user_agent", configuration["APIFlash:UA"] },
                {"ttl", "0" },
                {"fresh", "True" },
            };
        }

        private static string QueryString(IDictionary<string, string> dict)
        {
            var list = new List<string>();
            foreach (var item in dict)
            {
                list.Add(item.Key + "=" + item.Value);
            }
            return string.Join("&", list);
        }

        private static string CreateTweetText()
        {
            return "Today's Overwatch Arcademodes - " + DateTime.Now.ToString("dddd, d MMMM") + "\n#overwatch #owarcade";
        }
        
        public async Task CreateScreenshot()
        {
            var fileInfo = new FileInfo(ImageConstants.IMG_OW_SCREENSHOT);

            try
            {
                var response = await Client.GetAsync(URL + QueryString(CreateHttpParams(_configuration)));
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Couldn't reach screenshot service APIFlash (Http code {response.StatusCode})");
                    throw new HttpRequestException();
                }

                await using var ms = await response.Content.ReadAsStreamAsync();
                await using var fs = File.Create(fileInfo.FullName);
                ms.Seek(0, SeekOrigin.Begin);
                await ms.CopyToAsync(fs);
            }
            catch (Exception e)
            {
                _logger.LogError($"CreateScreenshot failed  {e.Message}");
            }
        }

        public async Task Handle(Game overwatchType)
        {
            await CreateScreenshot();
            var media = _operations.UploadImageFromPath(ImageConstants.IMG_OW_SCREENSHOT);
            await _operations.PostTweetWithMedia(CreateTweetText(), media);
        }
    }
}
