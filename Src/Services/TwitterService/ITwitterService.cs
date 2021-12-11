using System.Threading.Tasks;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Constants;

namespace OWArcadeBackend.Services.TwitterService
{
    public interface ITwitterService
    {
        public Task Handle(Game overwatchType);
    }
}
