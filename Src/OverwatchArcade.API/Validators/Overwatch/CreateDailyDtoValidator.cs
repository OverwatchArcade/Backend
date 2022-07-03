using FluentValidation;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Validators.Overwatch;

public class CreateDailyDtoValidator : AbstractValidator<CreateDailyDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly int _amountOfTiles;
    public CreateDailyDtoValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _amountOfTiles = TileCount();

        RuleForEach(d => d.TileModes).SetValidator(new CreateTileModesDtoValidator(_unitOfWork));
        RuleFor(d => d.TileModes).Must(HasAllTiles).WithMessage($"Must have exactly {_amountOfTiles} amount of tiles. I either received too much/little or received duplicate TileIds.");
    }

    private int TileCount()
    {
        return int.Parse(_unitOfWork.ConfigRepository.Find(x => x.Key == ConfigKeys.OwTiles.ToString()).Single().Value ?? throw new InvalidOperationException());
    }

    private bool HasAllTiles(ICollection<CreateTileModeDto> tileModes)
    {
        return tileModes.GroupBy(x => x.TileId).Count() == _amountOfTiles;
    }
}