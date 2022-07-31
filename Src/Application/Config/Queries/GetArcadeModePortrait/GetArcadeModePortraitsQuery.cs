using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits;

namespace OverwatchArcade.Application.Config.Queries.GetArcadeModePortrait;

public record GetArcadeModePortraitsQuery : IRequest<IEnumerable<ArcadeModePortrait>>;

public class GetArcadeModePortraitsQueryHandler : IRequestHandler<GetArcadeModePortraitsQuery, IEnumerable<ArcadeModePortrait>>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;

    public GetArcadeModePortraitsQueryHandler(IMapper mapper, IApplicationDbContext context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<ArcadeModePortrait>> Handle(GetArcadeModePortraitsQuery request, CancellationToken cancellationToken)
    {
        return await _context.ArcadeModes
            .ProjectTo<ArcadeModePortrait>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}