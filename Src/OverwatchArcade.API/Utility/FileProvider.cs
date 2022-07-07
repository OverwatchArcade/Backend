namespace OverwatchArcade.API.Utility;

/// <summary>
/// Class is used to easily mock the file provider
/// </summary>
public class FileProvider : IFileProvider
{
    public IEnumerable<string> GetFiles(string path)
    {
        return Directory.GetFiles(path);
    }

    public ICollection<string> GetDirectories(string path)
    {
        return Directory.GetDirectories(path);
    }
}