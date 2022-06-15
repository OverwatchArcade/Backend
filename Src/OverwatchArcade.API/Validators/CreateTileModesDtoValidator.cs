using FluentValidation;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Validators;

public class CreateTileModesDtoValidator : AbstractValidator<CreateTileModeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly Game _overwatch;

    public CreateTileModesDtoValidator(IUnitOfWork unitOfWork, Game overwatchType)
    {
        _unitOfWork = unitOfWork;
        _overwatch = overwatchType;

        RuleFor(tm => tm.ArcadeModeId).Must(ArcadeModeExists).WithMessage(tm => $"ArcadeModeId {tm.ArcadeModeId} doesn't exist");
        RuleFor(tm => tm.LabelId).Must(LabelExists).WithMessage(tm => $"LabelId {tm.LabelId} doesn't exist");
    }

    private bool ArcadeModeExists(int arcadeModeId)
    {
        return _unitOfWork.OverwatchRepository.Exists(x => x.Id == arcadeModeId && x.Game == _overwatch);
    }

    private bool LabelExists(int labelId)
    {
        return _unitOfWork.LabelRepository.Exists(x => x.Id == labelId);
    }
}