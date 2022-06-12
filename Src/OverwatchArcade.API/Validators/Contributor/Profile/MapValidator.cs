using FluentValidation;
using Newtonsoft.Json;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.ContributorProfile.Game.Overwatch.Portraits;
using OverwatchArcade.Persistence.Persistence;

namespace OverwatchArcade.API.Validators.Contributor.Profile
{
    public class MapValidator : AbstractValidator<Map>
    {
        private readonly IUnitOfWork _unitOfWork;
        private List<Hero> _overwatchMaps = new(); 

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
            var config = _unitOfWork.ConfigRepository.SingleOrDefault(x => x.Key == ConfigKeys.OW_MAPS.ToString());
            if (config?.JsonValue != null)
            {
                _overwatchMaps = JsonConvert.DeserializeObject<List<Hero>>(config.JsonValue.ToString());
            }
        }

        private bool ExistsInDatabase(Map map)
        {
            var foundMap = _overwatchMaps.Find(x => x.Name == map.Name);
            if (foundMap == null)
            {
                return false;
            }
            
            return foundMap.Image.Equals(map.Image) && foundMap.Name.Equals(map.Name);
        }
    }
}