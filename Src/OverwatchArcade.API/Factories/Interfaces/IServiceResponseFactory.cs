using OverwatchArcade.API.Dtos;

namespace OverwatchArcade.API.Factories.Interfaces;

public interface IServiceResponseFactory<T>
{
    public ServiceResponse<T> Create(T data);
    public ServiceResponse<T> Error(int statusCode, string errorMessage);
    public ServiceResponse<T> Error(int statusCode, string[] errorMessages);
}