using FluentValidation;
using Newtonsoft.Json;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Constants;
using OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Entities.Overwatch;

namespace OverwatchArcade.Application.Contributor.Commands.SaveProfile;

public class SaveProfileCommandValidator : AbstractValidator<SaveProfileCommand>
{
    private readonly IApplicationDbContext _context;

    private List<ArcadeMode> _arcadeModes = new();
    private List<MapPortrait> _mapPortraits = new();
    private List<HeroPortrait> _heroPortraits = new();

    public SaveProfileCommandValidator(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        
        GetHeroesFromConfig();
        GetMapsFromConfig();
        GetArcadeModes();
        
        RuleFor(profile => profile.About).Must(x => x.Description.Length <= 500 ).WithMessage("Profile about has too much characters");
        RuleFor(profile => profile.Overwatch.ArcadeModes).Must(arcadeModePortraits => ExistsInList(arcadeModePortraits, _arcadeModes));
        RuleFor(profile => profile.Overwatch.Maps).Must(mapPortraits => ExistsInList(mapPortraits, _mapPortraits));
        RuleFor(profile => profile.Overwatch.Heroes).Must(heroPortraits => ExistsInList(heroPortraits, _heroPortraits));
    }


    private void GetHeroesFromConfig()
    {
        var config = _context.Config.FirstOrDefault(x => x.Key == ConfigKeys.OwHeroes);
        if (config?.JsonValue != null)
        {
            _heroPortraits = JsonConvert.DeserializeObject<List<HeroPortrait>>(config.JsonValue.ToString())!;
        }
    }
    
    private void GetMapsFromConfig()
    {
        var config = _context.Config.FirstOrDefault(x => x.Key == ConfigKeys.OwMaps);
        if (config?.JsonValue != null)
        {
            _mapPortraits = JsonConvert.DeserializeObject<List<MapPortrait>>(config.JsonValue.ToString())!;
        }
    }
    
    private void GetArcadeModes()
    {
        _arcadeModes =  _context.ArcadeModes.ToList();
    }

    private static bool ExistsInList(IEnumerable<string> submittedItems, IEnumerable<Portrait> collection)
    {
        return collection.Select(item => item.Name).Except(submittedItems).Any();
    }
    
    private static bool ExistsInList(IEnumerable<string> submittedItems, IEnumerable<ArcadeMode> collection)
    {
        return collection.Select(item => item.Name).Except(submittedItems).Any();
    }
}