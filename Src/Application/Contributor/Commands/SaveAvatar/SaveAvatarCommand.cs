using ImageMagick;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Constants;

namespace OverwatchArcade.Application.Contributor.Commands.SaveAvatar;

public record SaveAvatarCommand(Guid UserId, IFormFile Avatar) : IRequest<string>
{
    public Guid UserId { get; set; } = UserId;
    public IFormFile Avatar { get; set; } = Avatar;
}

public class SaveAvatarCommandHandler : IRequestHandler<SaveAvatarCommand, string>
{
    private readonly IFileProvider _fileProvider;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SaveAvatarCommandHandler> _logger;

    public SaveAvatarCommandHandler(IFileProvider fileProvider, IApplicationDbContext context, ILogger<SaveAvatarCommandHandler> logger)
    {
        _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> Handle(SaveAvatarCommand request, CancellationToken cancellationToken)
    {
        var contributor = await _context.Contributors.FirstAsync(c => c.Id.Equals(request.UserId), cancellationToken);
        
        var fileName = Path.GetRandomFileName() + Path.GetExtension(request.Avatar.FileName);
        var filePath = Path.GetFullPath(AppContext.BaseDirectory + ImageConstants.ProfileFolder + fileName);
        if (!_fileProvider.DirectoryExists(AppContext.BaseDirectory + ImageConstants.ProfileFolder))
        {
            _fileProvider.CreateDirectory(AppContext.BaseDirectory + ImageConstants.ProfileFolder);
        }

        try
        {
            await _fileProvider.CreateFile(filePath, request.Avatar);
        }
        catch (Exception e)
        {
            _logger.LogError($"Couldn't upload avatar: {e.Message}");
            throw;
        }
        
        DeleteOldAvatar(contributor);
        await CompressImage(filePath);

        return fileName;
    }

    private void DeleteOldAvatar(Domain.Entities.Contributor contributor)
    {
        if (contributor.HasDefaultAvatar()) return;
        
        var oldImage = Path.GetFullPath(AppContext.BaseDirectory + ImageConstants.ProfileFolder + contributor.Avatar);
        _fileProvider.DeleteFile(oldImage);
    }
    
    private async Task CompressImage(string filePath)
    {
        try
        {
            using var image = new MagickImage(filePath);
            var size = new MagickGeometry(250, 250)
            {
                IgnoreAspectRatio = true
            };

            image.Resize(size);
            await image.WriteAsync(filePath);

            var optimizer = new ImageOptimizer();
            optimizer.Compress(filePath);
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Compression failed but avatar uploaded {e.Message}");
        }
    }
}