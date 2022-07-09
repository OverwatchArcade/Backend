using FluentValidation;
using Newtonsoft.Json;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Validators.Contributor.Profile
{
    public class MapValidator : AbstractValidator<MapPortrait>
    {
        private readonly IUnitOfWork _unitOfWork;
        private List<HeroPortrait> _overwatchMaps = new(); 

        public MapValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            GetMapsFromConfig();

            RuleFor(overwatchMap => overwatchMap)
                .Must(ExistsInDatabase)
                .WithMessage(overwatchMap => $"Overwatch Map {overwatchMap.Name} doesn't seem to be valid");
        }

        private void GetMapsFromConfig()
        {
            var config = _unitOfWork.ConfigRepository.FirstOrDefault(x => x.Key == ConfigKeys.OwMaps.ToString());
            if (config?.JsonValue != null)
            {
                _overwatchMaps = JsonConvert.DeserializeObject<List<HeroPortrait>>(config.JsonValue.ToString())!;
            }
        }

        private bool ExistsInDatabase(MapPortrait mapPortrait)
        {
            var foundMap = _overwatchMaps.Find(x => x.Name == mapPortrait.Name);
            if (foundMap == null)
            {
                return false;
            }
            
            return foundMap.Image.Equals(mapPortrait.Image) && foundMap.Name.Equals(mapPortrait.Name);
        }
    }
}