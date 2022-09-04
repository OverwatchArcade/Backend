using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OverwatchArcade.Application.Common.Exceptions;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Constants;
using OverwatchArcade.Domain.Entities.ContributorInformation.Personal;

namespace OverwatchArcade.Application.Config.Queries.GetCountries;

public record GetCountriesQuery : IRequest<IEnumerable<Country>>;

public class GetCountriesQueryHandler : IRequestHandler<GetCountriesQuery, IEnumerable<Country>>
{
    private readonly IApplicationDbContext _context;

    public GetCountriesQueryHandler(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Country>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        var countries = await _context.Config
            .FirstOrDefaultAsync(c => c.Key.Equals(ConfigKeys.Countries), cancellationToken);
        if (countries?.JsonValue == null)
        {
            throw new ConfigNotFoundException($"Config {ConfigKeys.Countries} not found");
        }
        
        return JsonConvert.DeserializeObject<IEnumerable<Country>>(countries.JsonValue.ToString())!;
    }
}