using System;
using System.IO;

namespace PartyCli.Infrastructure.Persistence;

public class AppDataFilePathProvider : IFilePathProvider
{
    public string GetServerFilePath()
    {
        return Path.Combine(GetRootAppdataPath(), "servers.json");
    }

    private string GetRootAppdataPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var folder = Path.Combine(appData, "partycli");
        Directory.CreateDirectory(folder);
        return folder;
    }
}