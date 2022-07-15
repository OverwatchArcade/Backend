using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OverwatchArcade.Domain.Models;

namespace OverwatchArcade.API.Utility;

public class AuthenticationToken : IAuthenticationToken
{
    private const int TokenExpirationDays = 14;
    private readonly IConfiguration _configuration;

    public AuthenticationToken(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public string CreateJwtToken(Contributor contributor)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, contributor.Id.ToString()),
            new(ClaimTypes.Name, contributor.Username),
            new(ClaimTypes.Role, contributor.Group.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Token").Value)
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