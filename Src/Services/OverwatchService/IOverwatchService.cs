using System;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Overwatch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OWArcadeBackend.Services.OverwatchService
{
    public interface IOverwatchService
    {
        Task<ServiceResponse<DailyDto>> Submit(Daily daily, Game overwatchType, Guid userId);
        Task<ServiceResponse<DailyDto>> Undo(Game overwatchType, Guid userId);
        Task<ServiceResponse<DailyDto>> GetDaily();
        ServiceResponse<List<ArcadeModeDto>> GetArcadeModes();
        ServiceResponse<List<Label>> GetLabels();
    }
}
