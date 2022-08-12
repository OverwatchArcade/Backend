using FluentValidation;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Validators.Overwatch;

public class CreateDailyDtoValidator : AbstractValidator<CreateDailyDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateDailyDtoValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        RuleForEach(d => d.TileModes).SetValidator(new CreateTileModesDtoValidator(_unitOfWork));
        RuleFor(d => d.TileModes).MustAsync(async (tileModes, _) => await HasExpectedTileCount(tileModes)).WithMessage("Must have exactly the configured amount of tiles. I either received too much/little or received duplicate TileIds.");
    }

    private async Task<bool> HasExpectedTileCount(IEnumerable<CreateTileModeDto> tileModes)
    {
        var submittedTileCount = tileModes.GroupBy(x => x.TileId).Count();
        var config = await _unitOfWork.ConfigRepository.FirstOrDefaultASync(x => x.Key == ConfigKeys.OwTiles.ToString());
        var hasConfigSubmitted = int.TryParse(config?.Value, out var allowedTileCountValue);

        if (hasConfigSubmitted)
        {
            return submittedTileCount == allowedTileCountValue;
        }

        throw new ArgumentException("TileCount config value not set");
    }
}