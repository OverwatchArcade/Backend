using System;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Overwatch;
using System.Collections.Generic;
using System.Threading.Tasks;
using OWArcadeBackend.Dtos.Overwatch;
using OWArcadeBackend.Models.Constants;

namespace OWArcadeBackend.Services.OverwatchService
{
    public interface IOverwatchService
    {
        Task<ServiceResponse<DailyDto>> Submit(Daily daily, Game overwatchType, Guid userId);
        Task<ServiceResponse<DailyDto>> Undo(Game overwatchType, Guid userId, bool hardDelete);
        Task<ServiceResponse<DailyDto>> GetDaily();
        ServiceResponse<List<ArcadeModeDto>> GetArcadeModes();
        ServiceResponse<List<Label>> GetLabels();
    }
}
