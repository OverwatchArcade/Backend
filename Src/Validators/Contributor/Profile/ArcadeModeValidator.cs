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
            
            RuleFor(x => x).Must(ArcadeModeExists).WithMessage(x => $"ArcadeMode {x.Name} doesn't exist");
        }

        private bool ArcadeModeExists(ArcadeModeSettingDto arcadeMode)
        {
            var foundArcadeMode = _unitOfWork.OverwatchRepository.Find(x => x.Name == arcadeMode.Name && x.Game == _overwatch).FirstOrDefault();
            if (foundArcadeMode == null)
            {
                return false;
            }
            
            var fullImageUrl = Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwArcadeFolder + foundArcadeMode.Image;
            return fullImageUrl.Equals(arcadeMode.Image);
        }
    }
}