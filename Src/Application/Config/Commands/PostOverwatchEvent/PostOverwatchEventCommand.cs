using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Application.Config.Commands.PostOverwatchEvent;

public record PostOverwatchEventCommand(string Event) : IRequest
{
    public string Event { get; set; } = Event;
}

public class PostOverwatchEventCommandHandler : IRequestHandler<PostOverwatchEventCommand>
{
    private readonly IApplicationDbContext _context;

    public PostOverwatchEventCommandHandler(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<Unit> Handle(PostOverwatchEventCommand request, CancellationToken cancellationToken)
    {
        var config = await _context.Config.FirstAsync(config => config.Key.Equals(ConfigKeys.OwCurrentEvent.ToString()), cancellationToken);
        config.Value = request.Event;

        await _context.SaveASync(cancellationToken);

        return Unit.Value;
    }
}