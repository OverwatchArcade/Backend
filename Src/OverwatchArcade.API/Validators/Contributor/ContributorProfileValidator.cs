using FluentValidation;
using OverwatchArcade.API.Validators.Contributor.Profile;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Validators.Contributor
{
    public class ContributorProfileValidator : AbstractValidator<ContributorProfile>
    {

        public ContributorProfileValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(d => d.Personal).Must(x => x.Text?.Length <= 500 ).WithMessage($"Profile about has too much characters");
            
            RuleForEach(d => d.Game.Overwatch.ArcadeModes).SetValidator(new ArcadeModeValidator(unitOfWork, Game.OVERWATCH));
            RuleForEach(d => d.Game.Overwatch.Heroes).SetValidator(new HeroValidator(unitOfWork));
            RuleForEach(d => d.Game.Overwatch.Maps).SetValidator(new MapValidator(unitOfWork));
        }
    }
}