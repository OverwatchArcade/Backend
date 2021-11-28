using Newtonsoft.Json.Linq;

namespace OWArcadeBackend.Models.Twitter
{
    public class Media
    {
        public enum MediaCategory
        {
            TWEET_IMAGE,
            AMPLIFY_VIDEO,
            TWEET_GIF,
            TWEET_VIDEO
        }

        private JObject MediaData { get; set; }

        public Media(JObject mediaData)
        {
            MediaData = mediaData;
        }

        public string MediaId => MediaData["media_id"].ToString();

        public string MediaKey
        {
            get
            {
                var data = MediaData["media_key"];
                return data != null ? data.ToString() : "";
            }
        }
    }
}
