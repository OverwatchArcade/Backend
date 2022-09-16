using OverwatchArcade.Application.Common.Interfaces;

namespace OverwatchArcade.Application.Common.Utility;

/// <summary>
/// Class is used to easily mock the file provider
/// </summary>
public class FileProvider : IFileProvider
{
    public IEnumerable<string> GetFiles(string path)
    {
        return Directory.GetFiles(path);
    }
    
    public async Task CreateFile(string path, byte[] fileContent)
    {
        await File.WriteAllBytesAsync(path, fileContent);
    }

    public void DeleteFile(string path)
    {
        File.Delete(path);
    }

    public ICollection<string> GetDirectories(string path)
    {
        return Directory.GetDirectories(path);
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }
}