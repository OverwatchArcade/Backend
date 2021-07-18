using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using static OWArcadeBackend.Models.Twitter.APIHandler;

namespace OWArcadeBackend.Models.Twitter
{

    public class Operations : IOperations
    {
        private readonly ILogger<Operations> _logger;
        static readonly int tweet_limit = 280;

        public IAPIHandler APIHandler;

        public Operations(ILogger<Operations> logger, IAPIHandler apiHandler)
        {
            _logger = logger;
            this.APIHandler = apiHandler;
        }

        /// <summary>
        /// Upload Image from path
        /// </summary>
        public Media UploadImageFromPath(string path)
        {
            byte[] dataBytes = File.ReadAllBytes(path);
            string encodedFileAsBase64 = Convert.ToBase64String(dataBytes);

            var parameters = new Dictionary<string, object> { };
            parameters.Add("media_data", encodedFileAsBase64);
            parameters.Add("media_category", Media.MediaCategory.tweet_image);

            try
            {
                var response = APIHandler.requestAPIOAuthAsync("https://upload.twitter.com/1.1/media/upload.json", Method.POST, parameters);
                var media_data = new Media(JObject.Parse(response.Result));
                return media_data;
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

            var parameters = new Dictionary<string, object> { };
            parameters.Add("status", text);

            try
            {
                await APIHandler.requestAPIOAuthAsync("https://api.twitter.com/1.1/statuses/update.json?media_ids=" + media.Media_id, Method.POST, parameters);
            }
            catch (Exception e)
            {
                _logger.LogError($"Couldn't post tweet: {e.Message}");
            }
        }

    }
}
