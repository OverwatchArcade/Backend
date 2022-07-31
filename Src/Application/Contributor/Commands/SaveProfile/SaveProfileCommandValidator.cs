using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Entities.Overwatch;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Application.Contributor.Commands.SaveProfile;

public class SaveProfileCommandValidator : AbstractValidator<SaveProfileCommand>
{
    private readonly IApplicationDbContext _context;

    private List<ArcadeMode> _arcadeModes;
    private List<MapPortrait> _mapPortraits;
    private List<HeroPortrait> _heroPortraits;

    public SaveProfileCommandValidator(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        
        GetHeroesFromConfig();
        GetMapsFromConfig();
        GetArcadeModes();
        
        RuleFor(profile => profile.Personal).Must(x => x.Text.Length <= 500 ).WithMessage($"Profile about has too much characters");
        RuleFor(profile => profile.Overwatch.Heroes);
    }


    private void GetHeroesFromConfig()
    {
        var config = _context.Config.FirstOrDefault(x => x.Key == ConfigKeys.OwHeroes.ToString());
        if (config?.JsonValue != null)
        {
            _heroPortraits = JsonConvert.DeserializeObject<List<HeroPortrait>>(config.JsonValue.ToString())!;
        }
    }
    
    private void GetMapsFromConfig()
    {
        var config = _context.Config.FirstOrDefault(x => x.Key == ConfigKeys.OwMaps.ToString());
        if (config?.JsonValue != null)
        {
            _mapPortraits = JsonConvert.DeserializeObject<List<MapPortrait>>(config.JsonValue.ToString())!;
        }
    }
    
    private void GetArcadeModes()
    {
        _arcadeModes =  _context.ArcadeModes.ToList();
    }

    private bool ExistsInDatabase(HeroPortrait heroPortrait)
    {
        var foundHero = _overwatchHeroes.Find(x => x.Name == heroPortrait.Name);
        if (foundHero == null)
        {
            return false;
        }

        return foundHero.Image.Equals(heroPortrait.Image) && foundHero.Name.Equals(heroPortrait.Name);
    }
}