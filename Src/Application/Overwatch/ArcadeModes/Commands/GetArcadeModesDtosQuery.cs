using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Entities.Overwatch;

namespace OverwatchArcade.Application.Overwatch.ArcadeModes.Commands;

public record GetArcadeModesDtosQuery : IRequest<ICollection<ArcadeModeDto>>;

public class GetArcadeModesDtosHandler : IRequestHandler<GetArcadeModesDtosQuery, ICollection<ArcadeModeDto>>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;

    public GetArcadeModesDtosHandler(IMapper mapper, IApplicationDbContext context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ICollection<ArcadeModeDto>> Handle(GetArcadeModesDtosQuery request, CancellationToken cancellationToken)
    {
        return await _context.ArcadeModes
            .AsNoTracking()
            .ProjectTo<ArcadeModeDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
