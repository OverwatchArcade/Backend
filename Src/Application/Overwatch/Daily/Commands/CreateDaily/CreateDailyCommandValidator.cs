using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Exceptions;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Constants;

namespace OverwatchArcade.Application.Overwatch.Daily.Commands.CreateDaily;

public class CreateDailyCommandValidator : AbstractValidator<CreateDailyCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateDailyCommandValidator(IApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        
        RuleFor(cd => cd.TileModes).MustAsync(async (tiles, _) => await HasAllTiles(tiles)).WithMessage("Daily must match required amount of tiles");
    }
    
    private async Task<bool> HasAllTiles(IEnumerable<CreateTileModeDto> tileModes)
    {
        var config = await _context.Config.FirstOrDefaultAsync(c => c.Key.Equals(ConfigKeys.OwTiles));
        if (config is null)
        {
            throw new ConfigNotFoundException($"Config key {ConfigKeys.OwTiles} not found");
        }

        var isNumber = int.TryParse(config.Value, out var amountOfTiles);
        if (!isNumber)
        {
            throw new ConfigNotFoundException($"Config key {ConfigKeys.OwTiles} has no number value");
        }

        return tileModes.GroupBy(x => x.TileId).Count() == amountOfTiles;
    }
}