using FluentValidation;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Validators.Contributor.Profile
{
    public class ArcadeModeValidator : AbstractValidator<ArcadeModePortrait>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ArcadeModeValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            
            RuleFor(overwatchArcade => overwatchArcade)
                .Must(ExistsInDatabase)
                .WithMessage(overwatchArcade => $"Overwatch Arcade {overwatchArcade.Name} doesn't seem to be valid");
        }

        private bool ExistsInDatabase(ArcadeModePortrait arcadeModePortrait)
        {
            var foundArcadeMode = _unitOfWork.OverwatchRepository.Find(x => x.Name == arcadeModePortrait.Name).FirstOrDefault();
            if (foundArcadeMode == null)
            {
                return false;
            }
            
            return foundArcadeMode.Image.Equals(arcadeModePortrait.Image) && foundArcadeMode.Name.Equals(arcadeModePortrait.Name);
        }
    }
}