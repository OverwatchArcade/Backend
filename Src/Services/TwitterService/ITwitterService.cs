using OWArcadeBackend.Models.Overwatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Services.Twitter
{
    public interface ITwitterService
    {
        public Task CreateScreenshot();
        public Task Handle(Game overwatchType);
    }
}
