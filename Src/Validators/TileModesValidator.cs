﻿using FluentValidation;
using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Persistence;
using System.Linq;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Validators
{
    public class TileModesValidator : AbstractValidator<TileMode>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Game _overwatch;

        public TileModesValidator(IUnitOfWork unitOfWork, Game overwatchType)
        {
            _unitOfWork = unitOfWork;
            _overwatch = overwatchType;

            RuleFor(tm => tm.ArcadeModeId).Must(ArcadeModeExists).WithMessage(tm => $"ArcadeModeId {tm.ArcadeModeId} doesn't exist");
            RuleFor(tm => tm.LabelId).Must(LabelExists).WithMessage(tm => $"LabelId {tm.LabelId} doesn't exist");
        }

        private bool ArcadeModeExists(int arcadeModeId)
        {
            return _unitOfWork.OverwatchRepository.Find(x => x.Id == arcadeModeId && x.Game == _overwatch).Any();
        }

        private bool LabelExists(int labelId)
        {
            return _unitOfWork.LabelRepository.Find(x => x.Id == labelId).Any();
        }
    }
}
