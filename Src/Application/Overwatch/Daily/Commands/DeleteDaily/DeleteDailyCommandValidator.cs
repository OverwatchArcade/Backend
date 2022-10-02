using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;

namespace OverwatchArcade.Application.Overwatch.Daily.Commands.DeleteDaily;

public class DeleteDailyCommandValidator : AbstractValidator<DeleteDailyCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteDailyCommandValidator(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    {
        _context = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        var userService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        
        RuleFor(cd => cd).MustAsync(async (_, _) => await IsAuthorised(userService.UserId))
            .WithName("User")
            .WithMessage("User is not authorised");
        
        RuleFor(daily => daily).MustAsync(async (_, _) => await HasDailySubmittedToday())
            .WithName("Daily")
            .WithMessage("No daily submitted yet");
    }
    
    private async Task<bool> IsAuthorised(Guid userId)
    {
        return await _context.Contributors.Where(c => c.Id.Equals(userId)).AnyAsync();
    }
    
    private async Task<bool> HasDailySubmittedToday()
    {
        return await _context.Dailies
            .Where(d => d.MarkedOverwrite.Equals(false))
            .Where(d => d.CreatedAt >= DateTime.UtcNow.Date)
            .AnyAsync();
    }
}