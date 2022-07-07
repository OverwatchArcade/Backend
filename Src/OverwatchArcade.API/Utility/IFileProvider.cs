namespace OverwatchArcade.API.Utility;

public interface IFileProvider
{
    IEnumerable<string> GetFiles(string path);
    ICollection<string> GetDirectories(string path);
}