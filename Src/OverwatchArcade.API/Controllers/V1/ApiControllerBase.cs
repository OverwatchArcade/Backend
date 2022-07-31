using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OverwatchArcade.API.Controllers.V1;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
