using FluentValidation;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Validators.Contributor.Profile;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Validators.Contributor
{
    public class ContributorProfileValidator : AbstractValidator<ContributorProfileDto>
    {

        public ContributorProfileValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(profile => profile.Personal).Must(x => x.Text?.Length <= 500 ).WithMessage($"Profile about has too much characters");
            
            RuleForEach(profile => profile.Game.Overwatch.ArcadeModes).SetValidator(new ArcadeModeValidator(unitOfWork, Game.OVERWATCH));
            RuleForEach(profile => profile.Game.Overwatch.Heroes).SetValidator(new HeroValidator(unitOfWork));
            RuleForEach(profile => profile.Game.Overwatch.Maps).SetValidator(new MapValidator(unitOfWork));
        }
    }
}