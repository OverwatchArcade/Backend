using FluentValidation;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Validators
{
    public class TileModesValidator : AbstractValidator<TileMode>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TileModesValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(tm => tm.ArcadeModeId).Must(ArcadeModeExists).WithMessage(tm => $"ArcadeModeId {tm.ArcadeModeId} doesn't exist");
            RuleFor(tm => tm.LabelId).Must(LabelExists).WithMessage(tm => $"LabelId {tm.LabelId} doesn't exist");
        }

        private bool ArcadeModeExists(int arcadeModeId)
        {
            return _unitOfWork.OverwatchRepository.Exists(x => x.Id == arcadeModeId);
        }

        private bool LabelExists(int labelId)
        {
            return _unitOfWork.LabelRepository.Exists(x => x.Id == labelId);
        }
    }
}
