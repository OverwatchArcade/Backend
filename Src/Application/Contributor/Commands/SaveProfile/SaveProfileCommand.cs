using MediatR;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Entities.ContributorInformation;
using OverwatchArcade.Domain.Entities.ContributorInformation.Game;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Application.Contributor.Commands.SaveProfile;


public record SaveProfileCommand : IRequest
{
    public About Personal { get; init; }
    public Socials Social { get; set; }
    public OverwatchProfile Overwatch { get; set; }
}

public class SaveProfileCommandHandler : IRequestHandler<SaveProfileCommand>
{
    private readonly IMemoryCache _memoryCache;
    private readonly IApplicationDbContext _context;

    public SaveProfileCommandHandler(IMemoryCache memoryCache, IApplicationDbContext context)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Unit> Handle(SaveProfileCommand request, CancellationToken cancellationToken)
    {
        var contributor = _context.Contributors.Where()
    }
}