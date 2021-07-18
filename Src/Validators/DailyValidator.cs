using System;
using FluentValidation;
using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Persistence;
using System.Collections.Generic;
using System.Linq;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Validators
{
    public class DailyValidator : AbstractValidator<Daily>
    {
        private readonly IUnitOfWork _unitOfWork;

        private const ConfigKeys OW_TILES_CONFIG_KEY = ConfigKeys.OW_TILES;
        private const ConfigKeys OW2_TILES_CONFIG_KEY = ConfigKeys.OW2_TILES;

        private readonly int _amountOfTiles;
        private readonly Game _overwatch;

        public DailyValidator(IUnitOfWork unitOfWork, Game overwatchType)
        {
            _unitOfWork = unitOfWork;
            _overwatch = overwatchType;
            _amountOfTiles = TileCount();

            RuleForEach(d => d.TileModes).SetValidator(new TileModesValidator(_unitOfWork, overwatchType));
            RuleFor(d => d.TileModes).Must(HasAllTiles).WithMessage($"{overwatchType.ToString()} Must have exactly {_amountOfTiles} amount of tiles. I either received too much/little or received duplicate TileIds.");
        }

        private int TileCount()
        {
            string configkey;
            configkey = _overwatch == Game.OVERWATCH ? OW_TILES_CONFIG_KEY.ToString() : OW2_TILES_CONFIG_KEY.ToString();

            return int.Parse(_unitOfWork.ConfigRepository.Find(x => x.Key == configkey).Single().Value ?? throw new InvalidOperationException());
        }

        private bool HasAllTiles(ICollection<TileMode> tileModes)
        {
            return tileModes.GroupBy(x => x.TileId).Count() == _amountOfTiles;
        }
    }
}
