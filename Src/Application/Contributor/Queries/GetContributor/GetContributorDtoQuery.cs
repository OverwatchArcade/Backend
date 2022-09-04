using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Application.Contributor.Queries.GetContributors;

namespace OverwatchArcade.Application.Contributor.Queries.GetContributor;

public record GetContributorDtoQuery : IRequest<ContributorDto?>
{
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
}

public class GetContributorDtoQueryHandler : IRequestHandler<GetContributorDtoQuery, ContributorDto?>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;

    public GetContributorDtoQueryHandler(IMapper mapper, IApplicationDbContext context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ContributorDto?> Handle(GetContributorDtoQuery request, CancellationToken cancellationToken)
    {
        var contributors = _context.Contributors.AsQueryable();

        if (request.UserId is not null)
        {
            contributors = contributors.Where(c => c.Id.Equals(request.UserId));
        }
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            contributors = contributors.Where(c => c.Email.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase));
        }
        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            contributors = contributors.Where(c => c.Username.Equals(request.Username, StringComparison.InvariantCultureIgnoreCase));
        }
        
        return await contributors
            .ProjectTo<ContributorDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}