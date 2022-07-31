using FluentValidation;

namespace OverwatchArcade.Application.Overwatch.Daily.Commands.CreateDaily;

public class CreateDailyCommandValidator : AbstractValidator<CreateDailyCommand>
{
    private const int AmountOfTiles = 7;

    public CreateDailyCommandValidator()
    {
        RuleFor(cd => cd.TileModes).Must(HasAllTiles).WithMessage($"Daily must have {AmountOfTiles} tiles");
    }
    
    private bool HasAllTiles(ICollection<CreateTileModeDto> tileModes)
    {
        return tileModes.GroupBy(x => x.TileId).Count() == AmountOfTiles;
    }
}