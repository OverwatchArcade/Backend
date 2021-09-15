using System.Threading.Tasks;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Services.TwitterService
{
    public interface ITwitterService
    {
        public Task Handle(Game overwatchType);
    }
}
