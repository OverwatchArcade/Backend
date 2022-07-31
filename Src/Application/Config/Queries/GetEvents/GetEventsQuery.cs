using MediatR;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Constants;

namespace OverwatchArcade.Application.Config.Queries.GetEvents;

public record GetEventsQuery : IRequest<string?[]>;

public class GetEventQueryHandler : IRequestHandler<GetEventsQuery, string?[]>
{
    private readonly IFileProvider _fileProvider;

    public GetEventQueryHandler(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
    }

    public async Task<string?[]> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(_fileProvider.GetDirectories(ImageConstants.OwEventsFolder)
            .Select(Path.GetFileName)
            .ToArray());
    }
}