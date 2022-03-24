using System;
using System.Collections.Generic;
using FluentValidation;
using Newtonsoft.Json;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Constants;
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
                _overwatchMaps = JsonConvert.DeserializeObject<List<ConfigOverwatchMap>>(config.JsonValue.ToString());
            }
        }

        private bool ExistsInDatabase(ConfigOverwatchMap map)
        {
            var foundMap = _overwatchMaps.Find(x => x.Name == map.Name);
            if (foundMap == null)
            {
                return false;
            }

            var foundMapImageUrl = ImageConstants.OwMapsFolder + foundMap.Image;
            return foundMapImageUrl.Equals(map.Image) && foundMap.Name.Equals(map.Name);
        }
    }
}