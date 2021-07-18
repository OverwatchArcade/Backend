using System.Linq;
using System.Runtime.Intrinsics.X86;
using FluentValidation;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Persistence;

namespace OWArcadeBackend.Validators
{
    public class RegisterValidator : AbstractValidator<DiscordLoginDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Username).NotEmpty().MinimumLength(2).MaximumLength(16).Must(IsUniqueUsername).WithMessage("Username is already taken");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().Must(IsUniqueEmail).WithMessage("Email is already registered");
            RuleFor(x => x.Id).NotEmpty().Must(IsWhitelisted).WithMessage(x => $"User id {x.Id} is not whitelisted");
        }

        private bool IsUniqueUsername(string username)
        {
            return !_unitOfWork.ContributorRepository.Find(x => x.Username.Equals(username)).Any();
        }

        private bool IsUniqueEmail(string email)
        {
            return !_unitOfWork.ContributorRepository.Find(x => x.Email.Equals(email)).Any();
        }

        private bool IsWhitelisted(string id)
        {
            return _unitOfWork.WhitelistRepository.IsDiscordWhitelisted(id);
        }
    }
}