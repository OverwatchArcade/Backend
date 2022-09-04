using MediatR;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Entities.ContributorInformation;
using OverwatchArcade.Domain.Entities.ContributorInformation.Game;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Application.Contributor.Commands.SaveProfile;


public record SaveProfileCommand : IRequest
{
    public Guid UserId { get; set; }
    public About Personal { get; init; }
    public Socials Social { get; set; }
    public OverwatchProfile Overwatch { get; set; }
}

public class SaveProfileCommandHandler : IRequestHandler<SaveProfileCommand>
{
    private readonly IApplicationDbContext _context;

    public SaveProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Unit> Handle(SaveProfileCommand request, CancellationToken cancellationToken)
    {
        var contributor = _context.Contributors.FirstOrDefault(c => c.Id.Equals(request.UserId));
        if (contributor is null)
        {
            return Unit.Value;
        }

        contributor.Profile = new ContributorProfile()
        {
            Personal = request.Personal,
            Social = request.Social,
            Overwatch = request.Overwatch
        };
        await _context.SaveASync(cancellationToken);
        
        return Unit.Value;
    }
}