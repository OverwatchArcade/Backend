namespace OverwatchArcade.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string WebRootPath { get; }
}
