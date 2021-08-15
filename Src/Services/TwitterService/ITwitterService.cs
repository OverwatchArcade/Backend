using System.Threading.Tasks;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Services.TwitterService
{
    public interface ITwitterService
    {
        public Task CreateScreenshot();
        public Task Handle(Game overwatchType);
    }
}
