using OverwatchArcade.Domain.Models;

namespace OverwatchArcade.API.Utility.Interfaces;

public interface IAuthenticationToken
{
    public string CreateJwtToken(Contributor contributor);
}