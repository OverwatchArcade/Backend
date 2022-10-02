using System.Security.Claims;
using OverwatchArcade.Application.Common.Interfaces;

namespace WebAPI.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
    {
        _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public Guid UserId => Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Couldn't parse userId to Guid"));
    public string WebRootPath => _webHostEnvironment.WebRootPath;
}
