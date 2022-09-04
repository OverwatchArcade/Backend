using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;

namespace OverwatchArcade.Application.Overwatch.Daily.Commands.DeleteDaily;

public class DeleteDailyCommandValidator : AbstractValidator<DeleteDailyCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteDailyCommandValidator(IApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        
        RuleFor(daily => daily).MustAsync(async (_, _) => await HasDailySubmittedToday()).WithMessage("No daily submitted yet");
    }
    
    private async Task<bool> HasDailySubmittedToday()
    {
        return await _context.Dailies
            .Where(d => d.MarkedOverwrite.Equals(false))
            .Where(d => d.CreatedAt >= DateTime.UtcNow.Date)
            .AnyAsync();
    }
}