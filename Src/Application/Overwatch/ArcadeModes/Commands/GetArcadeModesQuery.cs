using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Entities.Overwatch;

namespace OverwatchArcade.Application.Overwatch.ArcadeModes.Commands;

public record GetArcadeModesQuery : IRequest<ICollection<ArcadeMode>>;

public class GetArcadeModesQueryHandler : IRequestHandler<GetArcadeModesQuery, ICollection<ArcadeMode>>
{
    private readonly IApplicationDbContext _context;
    
    public GetArcadeModesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<ArcadeMode>> Handle(GetArcadeModesQuery request, CancellationToken cancellationToken)
    {
        return await _context.ArcadeModes
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
