using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;

namespace OverwatchArcade.Application.Overwatch.Daily.Queries.GetDaily;

public record GetDailyQuery : IRequest<DailyDto>;

public class GetDailyQueryHandler : IRequestHandler<GetDailyQuery, DailyDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDailyQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DailyDto> Handle(GetDailyQuery request, CancellationToken cancellationToken)
    {
        var daily = await _context.Dailies
            .AsNoTracking()
            .Include(c => c.Contributor)
            .Include(c => c.TileModes)
            .ThenInclude(tile => tile.Label)
            .Include(c => c.TileModes)
            .ThenInclude(tile => tile.ArcadeMode)
            .OrderByDescending(d => d.Id)
            .FirstAsync(cancellationToken);

        var dailyDto = _mapper.Map<DailyDto>(daily);
        dailyDto.IsToday = daily.CreatedAt >= DateTime.UtcNow.Date && !daily.MarkedOverwrite;

        return dailyDto;
    }
}
