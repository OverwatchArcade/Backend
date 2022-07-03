using FluentValidation;
using OverwatchArcade.API.Dtos.Contributor;

namespace OverwatchArcade.API.Validators.Contributor;

public class ContributorAvatarValidator : AbstractValidator<ContributorAvatarDto>
{
    public ContributorAvatarValidator()
    {
        RuleFor(profile => profile!.Avatar.Length).NotNull().LessThanOrEqualTo(750000)
            .WithMessage("File size exceeds the 750kb limit");

        RuleFor(profile => profile!.Avatar.ContentType).NotNull()
            .Must(x => x.Equals("image/jpeg") || x.Equals("image/jpg") || x.Equals("image/png"))
            .WithMessage("File type is not allowed. Must be JPG/PNG");
    }
}