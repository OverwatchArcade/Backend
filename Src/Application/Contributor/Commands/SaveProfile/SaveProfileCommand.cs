using MediatR;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Constants;
using OverwatchArcade.Domain.Entities.ContributorInformation;
using OverwatchArcade.Domain.Entities.ContributorInformation.Game;
using OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits;

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

        var configs = _context.Config.ToList();
        var arcadeModePortraits = _context.ArcadeModes
            .Where(am => request.Overwatch.ArcadeModes.Contains(am.Name))
            .Select(am => new ArcadeModePortrait(am.Name, am.Image))
            .ToList();
        var heroPortraits = configs.Where(c => c.Key == ConfigKeys.OwHeroes).Select(c => new HeroPortrait(c.))
        
        
        var overwatchProfile = new OverwatchProfile()
        {
            ArcadeModes = arcadeModePortraits,
            
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