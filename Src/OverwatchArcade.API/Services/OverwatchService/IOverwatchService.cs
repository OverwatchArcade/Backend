using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.API.Services.OverwatchService
{
    public interface IOverwatchService
    {
        Task<ServiceResponse<DailyDto>> Submit(CreateDailyDto createDailyDto, Game overwatchType, Guid userId);
        Task<ServiceResponse<DailyDto>> Undo(Game overwatchType, Guid userId, bool hardDelete);
        ServiceResponse<DailyDto> GetDaily();
        ServiceResponse<List<ArcadeModeDto>> GetArcadeModes();
        ServiceResponse<List<Label>> GetLabels();
    }
}
