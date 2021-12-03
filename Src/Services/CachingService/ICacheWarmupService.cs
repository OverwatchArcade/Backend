using System.Threading.Tasks;

namespace OWArcadeBackend.Services.CachingService
{
    public interface ICacheWarmupService
    {
        public Task Run();
    }
}