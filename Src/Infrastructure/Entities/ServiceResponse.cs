namespace OverwatchArcade.Persistence.Entities;

public class ServiceResponse<T>
{
    public T Data { get; set; }
    public bool Success => !ErrorMessages.Any();
    public List<string> ErrorMessages { get; } = new();
    public int StatusCode { get; set; } = 200;
}