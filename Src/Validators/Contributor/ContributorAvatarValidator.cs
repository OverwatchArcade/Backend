using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace OWArcadeBackend.Validators.Contributor
{
    public class ContributorAvatarValidator : AbstractValidator<IFormFile>
    {
        public ContributorAvatarValidator()
        {
            RuleFor(x => x.Length).NotNull().LessThanOrEqualTo(750000)
                .WithMessage("File size exceeds the 750kb limit");

            RuleFor(x => x.ContentType).NotNull()
                .Must(x => x.Equals("image/jpeg") || x.Equals("image/jpg") || x.Equals("image/png"))
                .WithMessage("File type is not allowed. Must be JPG/PNG");
        }
    }
}