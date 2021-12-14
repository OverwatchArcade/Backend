using System.Threading.Tasks;
using OWArcadeBackend.Models.Constants;

namespace OWArcadeBackend.Services.TwitterService
{
    public interface ITwitterService
    {
        public Task PostTweet(Game overwatchType);
        public Task DeleteLastTweet();
    }
}
