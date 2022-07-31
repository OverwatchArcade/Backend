using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OverwatchArcade.Domain.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum ContributorGroup
{
    Contributor,
    Developer
}