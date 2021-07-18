using System;
using FluentValidation;
using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence;
using OWArcadeBackend.Validators.Contributor.Profile;

namespace OWArcadeBackend.Validators.Contributor
{
    public class ContributorProfileValidator : AbstractValidator<ContributorProfile>
    {

        public ContributorProfileValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(d => d.Personal).Must(x => x.About?.Length <= 500 ).WithMessage($"Profile about has too much characters");
            
            RuleForEach(d => d.Game.Overwatch.ArcadeModes).SetValidator(new ArcadeModeValidator(unitOfWork, Game.OVERWATCH));
            RuleForEach(d => d.Game.Overwatch.Heroes).SetValidator(new HeroValidator(unitOfWork));
            RuleForEach(d => d.Game.Overwatch.Maps).SetValidator(new MapValidator(unitOfWork));
        }
    }
}