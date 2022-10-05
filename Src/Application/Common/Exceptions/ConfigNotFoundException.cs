namespace OverwatchArcade.Application.Common.Exceptions;

public class ConfigNotFoundException : Exception
{
    public ConfigNotFoundException(string message) : base(message)
    {
    }
}
