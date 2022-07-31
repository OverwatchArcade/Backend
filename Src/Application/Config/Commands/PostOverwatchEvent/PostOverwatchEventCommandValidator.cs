using FluentValidation;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Constants;

namespace OverwatchArcade.Application.Config.Commands.PostOverwatchEvent;

public class PostOverwatchEventCommandValidator : AbstractValidator<PostOverwatchEventCommand>
{
    private readonly IFileProvider _fileProvider;
    
    public PostOverwatchEventCommandValidator(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));

        RuleFor(o => o.Event)
            .Must(EventExists)
            .WithMessage("Unknown event submitted");
    }

    private bool EventExists(string submittedEvent)
    {
        var events = _fileProvider.GetDirectories( ImageConstants.OwEventsFolder)
            .Select(Path.GetFileName)
            .ToList();

        return events.Exists(e => e.Equals(submittedEvent));
    }
}