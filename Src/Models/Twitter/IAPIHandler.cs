using System.Collections.Generic;
using System.Threading.Tasks;

namespace OWArcadeBackend.Models.Twitter
{
    public interface IApiHandler
    {
        public Task<string> RequestApioAuthAsync(string url, ApiHandler.Method method);
        public Task<string> RequestApioAuthAsync(string url, ApiHandler.Method method, Dictionary<string, object> body);
    }
}
