using AutoMapper;
using MediatR;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Entities.Overwatch;

namespace OverwatchArcade.Application.Overwatch.Daily.Commands.CreateDaily;

public record CreateDailyCommand(IEnumerable<CreateTileModeDto> TileModes) : IRequest
{
    public IEnumerable<CreateTileModeDto> TileModes { get; } = TileModes;
}

public class CreateDailyCommandHandler : IRequestHandler<CreateDailyCommand, Unit>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateDailyCommandHandler(IMapper mapper, IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<Unit> Handle(CreateDailyCommand request, CancellationToken cancellationToken)
    {
        var daily = new Domain.Entities.Overwatch.Daily
        {
            TileModes = _mapper.Map<IEnumerable<TileMode>>(request.TileModes),
            ContributorId = _currentUserService.UserId
        };

        _context.Dailies.Add(daily);
        await _context.SaveASync(cancellationToken);
        
        return Unit.Value;
    }
}