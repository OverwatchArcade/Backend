using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OverwatchArcade.Application.Common.Interfaces;

namespace WebAPI;

public class GenerateJwt : IGenerateJwt
{
    private const int TokenExpirationDays = 14;
    private readonly IConfiguration _configuration;

    public GenerateJwt(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public string CreateToken(OverwatchArcade.Domain.Entities.Contributor contributor)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, contributor.Id.ToString()),
            new(ClaimTypes.Name, contributor.Username),
            new(ClaimTypes.Role, contributor.Group.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Token"))
        );

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(TokenExpirationDays),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}