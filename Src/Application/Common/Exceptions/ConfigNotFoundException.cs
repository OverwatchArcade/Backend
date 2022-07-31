namespace OverwatchArcade.Application.Common.Exceptions;

public class ConfigNotFoundException : Exception
{
    public ConfigNotFoundException() : base() { }
    
    public ConfigNotFoundException(string message) : base(message)
    {
    }
}
