using FluentValidation;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Validators.Contributor.Profile
{
    public class ArcadeModeValidator : AbstractValidator<ArcadeModePortrait>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Game _overwatch;

        public ArcadeModeValidator(IUnitOfWork unitOfWork, Game overwatch)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _overwatch = overwatch;
            
            RuleFor(overwatchArcade => overwatchArcade)
                .Must(ExistsInDatabase)
                .WithMessage(overwatchArcade => $"Overwatch Arcade {overwatchArcade.Name} doesn't seem to be valid");
        }

        private bool ExistsInDatabase(ArcadeModePortrait arcadeModePortrait)
        {
            var foundArcadeMode = _unitOfWork.OverwatchRepository.Find(x => x.Name == arcadeModePortrait.Name && x.Game == _overwatch).FirstOrDefault();
            if (foundArcadeMode == null)
            {
                return false;
            }
            
            return foundArcadeMode.Image.Equals(arcadeModePortrait.Image) && foundArcadeMode.Name.Equals(arcadeModePortrait.Name);
        }
    }
}