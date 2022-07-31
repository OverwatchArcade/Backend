using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;

namespace OverwatchArcade.Application.Whitelist.Queries;

public record GetWhitelistQuery(string ProviderKey, string SocialProvider) : IRequest<bool>
{
    public string ProviderKey { get; set; } = ProviderKey;
    public string SocialProvider { get; set; } = SocialProvider;
}

public class GetWhitelistQueryHandler : IRequestHandler<GetWhitelistQuery, bool>
{
    private readonly IApplicationDbContext _context;
    public GetWhitelistQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(GetWhitelistQuery request, CancellationToken cancellationToken)
    {
        return await _context.Whitelist
            .Where(wl => wl.Provider.ToString().Equals(request.ProviderKey) && wl.ProviderKey.Equals(request.SocialProvider))
            .AnyAsync(cancellationToken: cancellationToken);
    }
}
