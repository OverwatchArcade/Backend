using OverwatchArcade.Domain.Models;

namespace OverwatchArcade.API.Utility;

public interface IAuthenticationToken
{
    public string CreateJwtToken(Contributor contributor);
}