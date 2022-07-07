using FluentValidation;
using Newtonsoft.Json;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Validators.Contributor.Profile
{
    public class HeroValidator : AbstractValidator<HeroPortrait>
    {
        private readonly IUnitOfWork _unitOfWork;
        private List<HeroPortrait> _overwatchHeroes = new(); 

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
            var config = _unitOfWork.ConfigRepository.FirstOrDefault(x => x.Key == ConfigKeys.OwHeroes.ToString());
            if (config?.JsonValue != null)
            {
                _overwatchHeroes = JsonConvert.DeserializeObject<List<HeroPortrait>>(config.JsonValue.ToString())!;
            }
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
}