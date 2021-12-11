﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OWArcadeBackend.Models.Constants;
using OWArcadeBackend.Models.Twitter;
using OWArcadeBackend.Services.ConfigService;

namespace OWArcadeBackend.Services.TwitterService
{
    public class TwitterService : ITwitterService
    {
        private readonly ILogger<TwitterService> _logger;
        private readonly IConfigService _configService;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IOperations _operations;
        private readonly IConfiguration _configuration;
        
        private const string ImgUrl = "https://api.apiflash.com/v1/urltoimage?";

        public TwitterService(ILogger<TwitterService> logger, IConfigService configService, IOperations operations, IConfiguration configuration, IHttpClientFactory  httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _operations = operations ?? throw new ArgumentNullException(nameof(operations));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        private static Dictionary<string, string> CreateHttpParams(IConfiguration configuration)
        {
            return new Dictionary<string, string>
            {
                {"access_key", configuration["APIFlash:Key"]},
                {"url", configuration["APIFlash:URL"] },
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

        private async Task<string> CreateTweetText()
        {
            var currentEvent = await _configService.GetCurrentOverwatchEvent();
            if (!currentEvent.Data.Equals("default", StringComparison.CurrentCultureIgnoreCase))
            {
                return $"Today's Overwatch Arcademodes, (Event: {currentEvent.Data}) - {DateTime.Now:dddd, d MMMM} \n#overwatch #owarcade";
            }
            return $"Today's Overwatch Arcademodes - {DateTime.Now:dddd, d MMMM} \n#overwatch #owarcade";
        }
        
        private async Task CreateScreenshot()
        {
            var fileInfo = new FileInfo(ImageConstants.OwScreenshot);

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(ImgUrl + QueryString(CreateHttpParams(_configuration)));
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
                throw;
            }
        }

        public async Task Handle(Game overwatchType)
        {
            await CreateScreenshot();
            var media = _operations.UploadImageFromPath(ImageConstants.OwScreenshot);
            await _operations.PostTweetWithMedia(await CreateTweetText(), media);
        }
    }
}
