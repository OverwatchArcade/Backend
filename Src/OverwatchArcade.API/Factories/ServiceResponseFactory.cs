using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Factories.Interfaces;

namespace OverwatchArcade.API.Factories;

public class ServiceResponseFactory<T> : IServiceResponseFactory<T>
{
    public ServiceResponse<T> Create(T data)
    {
        return new ServiceResponse<T>()
        {
            Data = data
        };
    }
    
    public ServiceResponse<T> Error(int statusCode, string errorMessage)
    {
        return new ServiceResponse<T>()
        {
            Success = false,
            StatusCode = statusCode,
            ErrorMessages = new[] { errorMessage }
        };
    }

    public ServiceResponse<T> Error(int statusCode, string[] errorMessages)
    {
        return new ServiceResponse<T>()
        {
            Success = false,
            StatusCode = statusCode,
            ErrorMessages = errorMessages
        };
    }
}