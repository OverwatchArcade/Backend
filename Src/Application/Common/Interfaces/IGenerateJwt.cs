namespace OverwatchArcade.Application.Common.Interfaces;

public interface IGenerateJwt
{
    public string CreateToken(Domain.Entities.Contributor contributor);
}