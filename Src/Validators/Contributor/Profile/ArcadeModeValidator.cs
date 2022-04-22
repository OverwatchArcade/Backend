using System;
using System.Linq;
using FluentValidation;
using OWArcadeBackend.Dtos.Contributor.Profile.Game.Overwatch.Portraits;
using OWArcadeBackend.Models.Constants;
using OWArcadeBackend.Persistence;

namespace OWArcadeBackend.Validators.Contributor.Profile
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