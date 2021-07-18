using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWArcadeBackend.Models.Twitter
{
    public class Media
    {
        public enum MediaCategory
        {
            tweet_image,
            amplify_video,
            tweet_gif,
            tweet_video
        }

        private JObject Media_data { get; set; }

        public Media(JObject media_data)
        {
            this.Media_data = media_data;
        }

        public string Media_id
        {
            get
            {
                return Media_data["media_id"].ToString();
            }
        }
        public string Media_key
        {
            get
            {
                var data = Media_data["media_key"];

                if (data != null)
                    return data.ToString();
                else
                    return "";
            }
        }

        public float Size_mb
        {
            get
            {
                return (int.Parse(Media_data["size"].ToString()) / 1024f) / 1024f;
            }
        }
    }
}
