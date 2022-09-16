using System.Reflection;
using MediatR;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Application.Common.Security;

namespace OverwatchArcade.Application.Common.Behaviour;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;

    public AuthorizationBehaviour(
        ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();
        var attributes = authorizeAttributes.ToList();
        
        if (attributes.Any() && _currentUserService.UserId == null)
        {
            throw new UnauthorizedAccessException();
        }
        
        return await next();
    }
}