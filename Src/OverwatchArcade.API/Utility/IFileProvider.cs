namespace OverwatchArcade.API.Utility;

public interface IFileProvider
{
    IEnumerable<string> GetFiles(string path);
    Task CreateFile(string path, IFormFile file);
    void DeleteFile(string path);
    ICollection<string> GetDirectories(string path);
    bool DirectoryExists(string path);
    void CreateDirectory(string path);
}