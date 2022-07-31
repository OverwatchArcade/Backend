using AutoMapper;
using MediatR;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Entities.Overwatch;

namespace OverwatchArcade.Application.Overwatch.Daily.Commands.CreateDaily;

public record CreateDailyCommand : IRequest<Domain.Entities.Overwatch.Daily>
{
    public ICollection<CreateTileModeDto> TileModes { get; set; } = new List<CreateTileModeDto>();
}

public class CreateDailyCommandHandler : IRequestHandler<CreateDailyCommand, Domain.Entities.Overwatch.Daily>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    
    public CreateDailyCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    public async Task<Domain.Entities.Overwatch.Daily> Handle(CreateDailyCommand request, CancellationToken cancellationToken)
    {
        var daily = new Domain.Entities.Overwatch.Daily
        {
            TileModes = _mapper.Map<ICollection<TileMode>>(request.TileModes)
        };

        _context.Dailies.Add(daily);

        await _context.SaveASync(cancellationToken);

        return daily;
    }
}