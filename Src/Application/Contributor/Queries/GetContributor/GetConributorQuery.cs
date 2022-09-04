using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Application.Contributor.Queries.GetContributors;

namespace OverwatchArcade.Application.Contributor.Queries.GetContributor;

public record GetContributorQuery : IRequest<Domain.Entities.Contributor?>
{
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
}

public class GetContributorQueryHandler : IRequestHandler<GetContributorQuery, Domain.Entities.Contributor?>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;

    public GetContributorQueryHandler(IMapper mapper, IApplicationDbContext context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Domain.Entities.Contributor?> Handle(GetContributorQuery request, CancellationToken cancellationToken)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}