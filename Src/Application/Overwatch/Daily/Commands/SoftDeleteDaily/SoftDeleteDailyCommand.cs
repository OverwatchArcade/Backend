using MediatR;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Application.Overwatch.Daily.Commands.SoftDeleteDaily;

public record SoftDeleteDailyCommand : IRequest;

public class SoftDeleteDailyCommandHandler : IRequestHandler<SoftDeleteDailyCommand>
{
    private readonly IMemoryCache _memoryCache;
    private readonly IApplicationDbContext _context;

    public SoftDeleteDailyCommandHandler(IMemoryCache memoryCache, IApplicationDbContext context)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Unit> Handle(SoftDeleteDailyCommand request, CancellationToken cancellationToken)
    {
        _memoryCache.Remove(CacheKeys.OverwatchDaily);

        foreach (var dailyOwMode in  _context.Dailies.Where(d => d.CreatedAt >= DateTime.UtcNow.Date))
        {
            dailyOwMode.MarkedOverwrite = true;
        }
        
        await _context.SaveASync(cancellationToken);
        
        return Unit.Value;
    }
}