namespace OverwatchArcade.API.Services.CachingService
{
    public interface ICacheWarmupService
    {
        public Task Run();
    }
}