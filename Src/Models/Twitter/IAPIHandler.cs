using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWArcadeBackend.Models.Twitter
{
    public interface IAPIHandler
    {
        public Task<string> requestAPIOAuthAsync(string url, APIHandler.Method method);
        public Task<string> requestAPIOAuthAsync(string url, APIHandler.Method method, Dictionary<string, object> body);
    }
}
