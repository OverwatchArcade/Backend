using System.Threading.Tasks;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Services.TwitterService
{
    public interface ITwitterService
    {
        public void CreateScreenshot();
        public Task Handle(Game overwatchType);
    }
}
