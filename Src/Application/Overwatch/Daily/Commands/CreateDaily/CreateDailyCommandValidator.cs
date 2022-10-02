using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Exceptions;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Constants;

namespace OverwatchArcade.Application.Overwatch.Daily.Commands.CreateDaily;

public class CreateDailyCommandValidator : AbstractValidator<CreateDailyCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateDailyCommandValidator(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    {
        _context = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        var userService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        
        RuleFor(cd => cd).MustAsync(async (_, _) => await IsAuthorised(userService.UserId))
            .WithName("User")
            .WithMessage("User is not authorised");

        RuleFor(cd => cd).MustAsync(async (_, _) => await HasNoDailySubmittedYet())
            .WithName("Daily")
            .WithMessage("Daily has already been submitted for today");
        
        RuleFor(cd => cd.TileModes).MustAsync(async (tiles, _) => await HasAllTiles(tiles))
            .WithName("Tiles")
            .WithMessage("Daily must match required amount of tiles");
    }

    private async Task<bool> IsAuthorised(Guid userId)
    {
        return await _context.Contributors.Where(c => c.Id.Equals(userId)).AnyAsync();
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

    private async Task<bool> HasNoDailySubmittedYet()
    {
        return !await _context.Dailies
            .Where(d => d.MarkedOverwrite.Equals(false))
            .Where(d => d.CreatedAt >= DateTime.UtcNow.Date)
            .AnyAsync();
    }
}