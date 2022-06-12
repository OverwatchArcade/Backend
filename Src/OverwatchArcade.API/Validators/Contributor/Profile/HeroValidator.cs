using FluentValidation;
using Newtonsoft.Json;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.ContributorProfile.Game.Overwatch.Portraits;
using OverwatchArcade.Persistence.Persistence;

namespace OverwatchArcade.API.Validators.Contributor.Profile
{
    public class HeroValidator : AbstractValidator<Hero>
    {
        private readonly IUnitOfWork _unitOfWork;
        private List<Hero> _overwatchHeroes = new(); 

        public HeroValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            GetHeroesFromConfig();

            RuleFor(overwatchHero => overwatchHero)
                .Must(ExistsInDatabase)
                .WithMessage(overwatchHero => $"Overwatch Hero {overwatchHero.Name} doesn't seem to be valid");
        }

        private void GetHeroesFromConfig()
        {
            var config = _unitOfWork.ConfigRepository.SingleOrDefault(x => x.Key == ConfigKeys.OW_HEROES.ToString());
            if (config?.JsonValue != null)
            {
                _overwatchHeroes = JsonConvert.DeserializeObject<List<Hero>>(config.JsonValue.ToString());
            }
        }

        private bool ExistsInDatabase(Hero hero)
        {
            var foundHero = _overwatchHeroes.Find(x => x.Name == hero.Name);
            if (foundHero == null)
            {
                return false;
            }

            return foundHero.Image.Equals(hero.Image) && foundHero.Name.Equals(hero.Name);
        }
    }
}