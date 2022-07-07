namespace OverwatchArcade.API.Utility;

public interface IFileProvider
{
    IEnumerable<string> GetFiles(string path);
    ICollection<string> GetDirectories(string path);
    bool DirectoryExists(string path);
    void CreateDirectory(string path);
}