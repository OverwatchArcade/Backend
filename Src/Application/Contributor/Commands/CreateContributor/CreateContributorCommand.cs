using MediatR;
using OverwatchArcade.Application.Common.Interfaces;

namespace OverwatchArcade.Application.Contributor.Commands.CreateContributor;

public record CreateContributorCommand : IRequest<Domain.Entities.Contributor>
{
    public string Email { get; set; }
    public string Username { get; set; }
}

public class CreateContributorCommandHandler : IRequestHandler<CreateContributorCommand, Domain.Entities.Contributor>
{
    private readonly IApplicationDbContext _context;

    public CreateContributorCommandHandler(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<Domain.Entities.Contributor> Handle(CreateContributorCommand request, CancellationToken cancellationToken)
    {
        var contributor = new Domain.Entities.Contributor
        {
            Email = request.Email,
            Username = request.Username
        };

        _context.Contributors.Add(contributor);

        await _context.SaveASync(cancellationToken);

        return contributor;
    }
}