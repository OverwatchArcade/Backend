﻿using FluentValidation;
using OverwatchArcade.Persistence.Persistence;
using OWArcadeBackend.Dtos.Discord;

namespace OverwatchArcade.API.Validators
{
    public class LoginValidator : AbstractValidator<DiscordLoginDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoginValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(x => x.Email).NotEmpty().EmailAddress().Must(Exists).WithMessage("Incorrect email and/or password");
        }

        private bool Exists(string email)
        {
            return _unitOfWork.ContributorRepository.Exists(x => x.Email.Equals(email));
        }
    }
}