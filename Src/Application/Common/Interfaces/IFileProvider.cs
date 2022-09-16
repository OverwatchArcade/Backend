namespace OverwatchArcade.Application.Common.Interfaces;

public interface IFileProvider
{
    IEnumerable<string> GetFiles(string path);
    Task CreateFile(string path, byte[] fileContent);
    void DeleteFile(string path);
    ICollection<string> GetDirectories(string path);
    bool DirectoryExists(string path);
    void CreateDirectory(string path);
}