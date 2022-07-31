using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Entities.Overwatch;

namespace OverwatchArcade.Application.Overwatch.Labels.Queries;

public record GetLabelsQuery : IRequest<IEnumerable<Label>>;

public class GetLabelsQueryHandler : IRequestHandler<GetLabelsQuery, IEnumerable<Label>>
{
    private readonly IApplicationDbContext _context;

    public GetLabelsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Label>> Handle(GetLabelsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Labels.AsNoTracking().ToListAsync(cancellationToken);
    }
}