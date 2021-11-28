using System.Threading.Tasks;

namespace OWArcadeBackend.Models.Twitter
{
    public interface IOperations
    {
        public Media UploadImageFromPath(string path);
        public Task PostTweetWithMedia(string text, Media media);
    }
}
