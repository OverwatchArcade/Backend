using System;
using System.Collections.Generic;
using FluentValidation;
using Newtonsoft.Json;
using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence;

namespace OWArcadeBackend.Validators.Contributor.Profile
{
    public class HeroValidator : AbstractValidator<ConfigOverwatchHero>
    {
        private readonly IUnitOfWork _unitOfWork;
        private List<ConfigOverwatchHero> _overwatchHeroes = new(); 

        public HeroValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            GetHeroesFromConfig();

            RuleFor(x => x).Must(ExistsInDatabase).WithMessage("Overwatch Hero doesn't seem to be valid");
        }

        private void GetHeroesFromConfig()
        {
            var config = _unitOfWork.ConfigRepository.SingleOrDefault(x => x.Key == ConfigKeys.OW_HEROES.ToString());
            if (config?.JsonValue != null)
            {
                _overwatchHeroes = JsonConvert.DeserializeObject<List<ConfigOverwatchHero>>(config.JsonValue.ToString());
            }
        }

        private bool ExistsInDatabase(ConfigOverwatchHero hero)
        {
            var foundHero = _overwatchHeroes.Find(x => x.Name == hero.Name);
            if (foundHero == null)
            {
                return false;
            }
            
            var fullImageUrl = Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwHeroesFolder + foundHero.Image;
            return fullImageUrl.Equals(hero.Image);
        }
    }
}