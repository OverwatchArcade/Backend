using MediatR;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Application.Overwatch.Daily.Commands.DeleteDaily;

public record DeleteDailyCommand : IRequest;

public class DeleteDailyCommandHandler : IRequestHandler<DeleteDailyCommand>
{
    private readonly IMemoryCache _memoryCache;
    private readonly IApplicationDbContext _context;

    public DeleteDailyCommandHandler(IMemoryCache memoryCache, IApplicationDbContext context)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Unit> Handle(DeleteDailyCommand request, CancellationToken cancellationToken)
    {
        _memoryCache.Remove(CacheKeys.OverwatchDaily);
        // Delete last tweet
        
        var dailies = _context.Dailies.Where(d => d.CreatedAt >= DateTime.UtcNow.Date);
        _context.Dailies.RemoveRange(dailies);
        await _context.SaveASync(cancellationToken);
        
        return Unit.Value;
    }
}