using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;

namespace OverwatchArcade.Application.Contributor.Queries.GetContributors;


public record GetContributorsQuery : IRequest<IEnumerable<ContributorDto>>;

public class GetContributorsQueryHandler : IRequestHandler<GetContributorsQuery, IEnumerable<ContributorDto>>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;

    public GetContributorsQueryHandler(IMapper mapper, IApplicationDbContext context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<ContributorDto>> Handle(GetContributorsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Contributors
            .AsNoTracking()
            .ProjectTo<ContributorDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}