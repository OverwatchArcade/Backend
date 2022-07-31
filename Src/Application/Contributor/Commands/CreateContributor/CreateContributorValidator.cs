using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;

namespace OverwatchArcade.Application.Contributor.Commands.CreateContributor;

public class CreateContributorValidator : AbstractValidator<CreateContributorCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateContributorValidator(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        RuleFor(c => c.Email)
            .MustAsync(async (email, cancellationToken) => await IsUniqueEmail(email, cancellationToken))
            .WithMessage("Email already registered");
        
        RuleFor(c => c.Username)
            .MustAsync(async (username, cancellationToken) => await IsUniqueUsername(username, cancellationToken))
            .WithMessage("Username already registered");
    }

    private async Task<bool> IsUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _context.Contributors
            .Where(c => c.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase))
            .AnyAsync(cancellationToken);
    }
    
    private async Task<bool> IsUniqueUsername(string username, CancellationToken cancellationToken)
    {
        return !await _context.Contributors
            .Where(c => c.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
            .AnyAsync(cancellationToken);
    }
}