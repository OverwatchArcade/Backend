using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using static OWArcadeBackend.Models.Twitter.ApiHandler;

namespace OWArcadeBackend.Models.Twitter
{

    public class Operations : IOperations
    {
        private readonly ILogger<Operations> _logger;
        static readonly int tweet_limit = 280;

        private readonly IApiHandler _apiHandler;

        public Operations(ILogger<Operations> logger, IApiHandler apiHandler)
        {
            _logger = logger;
            _apiHandler = apiHandler;
        }

        /// <summary>
        /// Upload Image from path
        /// </summary>
        public Media UploadImageFromPath(string path)
        {
            byte[] dataBytes = File.ReadAllBytes(path);
            string encodedFileAsBase64 = Convert.ToBase64String(dataBytes);

            var parameters = new Dictionary<string, object>
            {
                { "media_data", encodedFileAsBase64 },
                { "media_category", Media.MediaCategory.TWEET_IMAGE }
            };

            try
            {
                var response = _apiHandler.RequestApioAuthAsync("https://upload.twitter.com/1.1/media/upload.json", Method.POST, parameters);
                var mediaData = new Media(JObject.Parse(response.Result));
                return mediaData;
            }
            catch (Exception e)
            {
                _logger.LogError($"Couldn't upload Tweet media: {e.Message}");
            }

            return null;
        }

        /// <summary>
        /// Post a tweet with text and MediaData instance
        /// </summary>
        public async Task PostTweetWithMedia(string text, Media media)
        {
            text = string.Join("", text.ToCharArray().Take(tweet_limit));

            var parameters = new Dictionary<string, object>();
            parameters.Add("status", text);

            try
            {
                await _apiHandler.RequestApioAuthAsync("https://api.twitter.com/1.1/statuses/update.json?media_ids=" + media.MediaId, Method.POST, parameters);
            }
            catch (Exception e)
            {
                _logger.LogError($"Couldn't post tweet: {e.Message}");
            }
        }

    }
}
