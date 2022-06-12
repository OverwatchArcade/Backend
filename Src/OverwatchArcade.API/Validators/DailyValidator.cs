﻿using FluentValidation;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence.Persistence;
using OWArcadeBackend.Validators;

namespace OverwatchArcade.API.Validators
{
    public class DailyValidator : AbstractValidator<Daily>
    {
        private readonly IUnitOfWork _unitOfWork;

        private const ConfigKeys OwTilesConfigKey = ConfigKeys.OW_TILES;
        private const ConfigKeys Ow2TilesConfigKey = ConfigKeys.OW2_TILES;

        private readonly int _amountOfTiles;
        private readonly Game _overwatch;

        public DailyValidator(IUnitOfWork unitOfWork, Game overwatchType)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _overwatch = overwatchType;
            _amountOfTiles = TileCount();

            RuleForEach(d => d.TileModes).SetValidator(new TileModesValidator(_unitOfWork, overwatchType));
            RuleFor(d => d.TileModes).Must(HasAllTiles).WithMessage($"{overwatchType.ToString()} Must have exactly {_amountOfTiles} amount of tiles. I either received too much/little or received duplicate TileIds.");
        }

        private int TileCount()
        {
            var configkey = _overwatch == Game.OVERWATCH ? OwTilesConfigKey.ToString() : Ow2TilesConfigKey.ToString();
            return int.Parse(_unitOfWork.ConfigRepository.Find(x => x.Key == configkey).Single().Value ?? throw new InvalidOperationException());
        }

        private bool HasAllTiles(ICollection<TileMode> tileModes)
        {
            return tileModes.GroupBy(x => x.TileId).Count() == _amountOfTiles;
        }
    }
}