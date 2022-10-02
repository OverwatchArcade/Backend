using ImageMagick;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeTypes;
using OverwatchArcade.Application.Common.Exceptions;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Constants;

namespace OverwatchArcade.Application.Contributor.Commands.SaveAvatar;

public record SaveAvatarCommand(byte[] FileContent, string FileType) : IRequest<string>
{
    public byte[] FileContent { get; set; } = FileContent;
    public string FileType { get; set; } = FileType;
}

public class SaveAvatarCommandHandler : IRequestHandler<SaveAvatarCommand, string>
{
    private readonly IFileProvider _fileProvider;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SaveAvatarCommandHandler> _logger;

    public SaveAvatarCommandHandler(IFileProvider fileProvider, IApplicationDbContext context,
        ICurrentUserService currentUserService, ILogger<SaveAvatarCommandHandler> logger)
    {
        _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> Handle(SaveAvatarCommand request, CancellationToken cancellationToken)
    {
        var contributor = await _context.Contributors.FirstOrDefaultAsync(c => c.Id.Equals(_currentUserService.UserId), cancellationToken);
        if (contributor is null)
        {
            throw new NotFoundException("Contributor doesn't exist");
        }
        
        var fileExtension = MimeTypeMap.GetExtension(request.FileType);
        var fileName = Path.GetRandomFileName() + fileExtension;
        
        var filePath = Path.GetFullPath(_currentUserService.WebRootPath + ImageConstants.ProfileFolder + fileName);
        if (!_fileProvider.DirectoryExists(_currentUserService.WebRootPath + ImageConstants.ProfileFolder))
        {
            _fileProvider.CreateDirectory(_currentUserService.WebRootPath + ImageConstants.ProfileFolder);
        }

        try
        {
            await _fileProvider.CreateFile(filePath, request.FileContent);
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
        if (contributor.HasDefaultAvatar) return;
        
        var oldImage = Path.GetFullPath(_currentUserService.WebRootPath + ImageConstants.ProfileFolder + contributor.Avatar);
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