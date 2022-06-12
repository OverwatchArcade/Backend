using FluentValidation;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorProfile.Game.Overwatch.Portraits;
using OverwatchArcade.Persistence.Persistence;

namespace OverwatchArcade.API.Validators.Contributor.Profile
{
    public class ArcadeModeValidator : AbstractValidator<ArcadeMode>
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

        private bool ExistsInDatabase(ArcadeMode arcadeMode)
        {
            var foundArcadeMode = _unitOfWork.OverwatchRepository.Find(x => x.Name == arcadeMode.Name && x.Game == _overwatch).FirstOrDefault();
            if (foundArcadeMode == null)
            {
                return false;
            }
            
            return foundArcadeMode.Image.Equals(arcadeMode.Image) && foundArcadeMode.Name.Equals(arcadeMode.Name);
        }
    }
}