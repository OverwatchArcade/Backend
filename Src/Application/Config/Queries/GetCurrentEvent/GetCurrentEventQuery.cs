using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Constants;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Application.Config.Queries.GetCurrentEvent;

public record GetCurrentEventQuery : IRequest<string>;

public class GetCurrentEventQueryHandler : IRequestHandler<GetCurrentEventQuery, string>
{
    private readonly IApplicationDbContext _context;

    public GetCurrentEventQueryHandler(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<string> Handle(GetCurrentEventQuery request, CancellationToken cancellationToken)
    {
        var config = await _context.Config.FirstAsync(x => x.Key.Equals(ConfigKeys.OwCurrentEvent.ToString()), cancellationToken);
        return config.Value ?? OverwatchConstants.DefaultThemeFolder;
    }
}