using FluentValidation;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Validators.Overwatch;

public class CreateTileModesDtoValidator : AbstractValidator<CreateTileModeDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTileModesDtoValidator(IUnitOfWork unitOfWork)
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