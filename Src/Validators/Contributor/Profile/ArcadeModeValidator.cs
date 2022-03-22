using System;
using System.Linq;
using FluentValidation;
using OWArcadeBackend.Dtos.Contributor;
using OWArcadeBackend.Models.Constants;
using OWArcadeBackend.Persistence;

namespace OWArcadeBackend.Validators.Contributor.Profile
{
    public class ArcadeModeValidator : AbstractValidator<ArcadeModeSettingDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Game _overwatch;

        public ArcadeModeValidator(IUnitOfWork unitOfWork, Game overwatch)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _overwatch = overwatch;
            
            RuleFor(overwatchArcade => overwatchArcade)
                .Must(ExistsInDatabase)
                .WithMessage(overwatchArcade => $"Overwatch Hero {overwatchArcade.Name} doesn't  seem to be valid");
        }

        private bool ExistsInDatabase(ArcadeModeSettingDto arcadeMode)
        {
            var foundArcadeMode = _unitOfWork.OverwatchRepository.Find(x => x.Name == arcadeMode.Name && x.Game == _overwatch).FirstOrDefault();
            if (foundArcadeMode == null)
            {
                throw new ArgumentException($"Overwatch Map {arcadeMode.Name} config not found");
            }

            var foundArcadeModeImageUrl = ImageConstants.OwArcadeFolder + foundArcadeMode.Image;
            return foundArcadeModeImageUrl.Equals(arcadeMode.Image) && foundArcadeMode.Name.Equals(arcadeMode.Name);
        }
    }
}