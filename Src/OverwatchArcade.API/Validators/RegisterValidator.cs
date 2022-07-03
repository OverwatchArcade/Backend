using FluentValidation;
using OverwatchArcade.API.Dtos.Discord;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Validators
{
    public class RegisterValidator : AbstractValidator<DiscordLoginDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(x => x.Username).NotEmpty().MinimumLength(2).MaximumLength(16).Must(IsUniqueUsername).WithMessage("Username is already taken");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().Must(IsUniqueEmail).WithMessage("Email is already registered");
            RuleFor(x => x.Id).NotEmpty().Must(IsWhitelisted).WithMessage(x => $"User id {x.Id} is not whitelisted");
        }

        private bool IsUniqueUsername(string username)
        {
            return !_unitOfWork.ContributorRepository.Exists(x => x.Username.Equals(username));
        }

        private bool IsUniqueEmail(string email)
        {
            return !_unitOfWork.ContributorRepository.Exists(x => x.Email.Equals(email));
        }

        private bool IsWhitelisted(string id)
        {
            return _unitOfWork.WhitelistRepository.IsDiscordWhitelisted(id);
        }
    }
}