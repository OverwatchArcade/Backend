using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Constants;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Application.Config.Queries.GetWallpaper;

public record GetWallpaperQuery : IRequest<string>;

public class GetWallpaperQueryHandler : IRequestHandler<GetWallpaperQuery, string>
{
    private readonly IFileProvider _fileProvider;
    private readonly IApplicationDbContext _context;

    public GetWallpaperQueryHandler(IFileProvider fileProvider, IApplicationDbContext context)
    {
        _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<string> Handle(GetWallpaperQuery request, CancellationToken cancellationToken)
    {
        var config = await _context.Config.FirstAsync(x => x.Key.Equals(ConfigKeys.OwCurrentEvent.ToString()), cancellationToken);
        var currentEvent = config.Value ?? OverwatchConstants.DefaultThemeFolder;
        var files = _fileProvider.GetFiles(ImageConstants.OwEventsFolder + currentEvent).Select(Path.GetFileName).ToList();
        
        return files[new Random().Next(files.Count)]!;
    }
}