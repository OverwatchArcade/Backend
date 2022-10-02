using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;

namespace OverwatchArcade.Application.Contributor.Queries.GetContributor;

public record GetContributorQuery : IRequest<Domain.Entities.Contributor?>
{
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
}

public class GetContributorQueryHandler : IRequestHandler<GetContributorQuery, Domain.Entities.Contributor?>
{
    private readonly IApplicationDbContext _context;

    public GetContributorQueryHandler(IApplicationDbContext context)
    {
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
            contributors = contributors.Where(c => c.Email.Equals(request.Email));
        }
        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            contributors = contributors.Where(c => c.Username.Equals(request.Username));
        }
        
        return await contributors
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }
}