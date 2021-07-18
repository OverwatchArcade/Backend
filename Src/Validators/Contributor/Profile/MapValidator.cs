using System;
using System.Collections.Generic;
using FluentValidation;
using Newtonsoft.Json;
using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence;

namespace OWArcadeBackend.Validators.Contributor.Profile
{
    public class MapValidator : AbstractValidator<ConfigOverwatchMap>
    {
        private readonly IUnitOfWork _unitOfWork;
        private List<ConfigOverwatchMap> _overwatchMaps = new(); 

        public MapValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            GetHeroesFromConfig();

            RuleFor(x => x).Must(ExistsInDatabase).WithMessage("Overwatch Map doesn't seem to be valid");
        }

        private void GetHeroesFromConfig()
        {
            var config = _unitOfWork.ConfigRepository.SingleOrDefault(x => x.Key == ConfigKeys.OW_MAPS.ToString());
            _overwatchMaps = JsonConvert.DeserializeObject<List<ConfigOverwatchMap>>(config.JsonValue.ToString());
        }

        private bool ExistsInDatabase(ConfigOverwatchMap map)
        {
            var foundMap = _overwatchMaps.Find(x => x.Name == map.Name);
            if (foundMap == null)
            {
                return false;
            }
            
            var fullImageUrl = Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.IMG_OW_MAPS_FOLDER + foundMap.Image;
            return fullImageUrl.Equals(map.Image);
        }
    }
}