using System;
using System.Collections.Generic;
using FluentValidation;
using Newtonsoft.Json;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Constants;
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

            RuleFor(overwatchHero => overwatchHero)
                .Must(ExistsInDatabase)
                .WithMessage(overwatchHero => $"Overwatch Hero {overwatchHero.Name} doesn't  seem to be valid");
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
                throw new ArgumentException($"Overwatch Hero {hero.Name} config not found");
            }

            var foundHeroImageUrl = ImageConstants.OwHeroesFolder + foundHero.Image;
            return foundHeroImageUrl.Equals(hero.Image) && foundHero.Name.Equals(hero.Name);
        }
    }
}