namespace OverwatchArcade.Application.Common.Exceptions;

public class CacheWarmupException : Exception
{
    public CacheWarmupException() : base()
    {
    }

    public CacheWarmupException(string message) : base(message)
    {
    }
}