using MediatR;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Entities.ContributorInformation;

namespace OverwatchArcade.Application.Contributor.Commands.SaveProfile;

public record SaveProfileCommand : IRequest
{
    public About About { get; set; }
    public Socials Socials { get; set; }
    public SaveOverwatchProfileDto Overwatch { get; set; }
}

public class SaveProfileCommandHandler : IRequestHandler<SaveProfileCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public SaveProfileCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<Unit> Handle(SaveProfileCommand request, CancellationToken cancellationToken)
    {
        var contributor = _context.Contributors.FirstOrDefault(c => c.Id.Equals(_currentUserService.UserId));
        if (contributor is null)
        {
            return Unit.Value;
        }

        contributor.Profile = new ContributorProfile()
        {
            About = request.About,
            Social = request.Socials,
            Overwatch = request.Overwatch
        };
        await _context.SaveASync(cancellationToken);
        
        return Unit.Value;
    }
}