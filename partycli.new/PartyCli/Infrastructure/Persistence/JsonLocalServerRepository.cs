using System.Text.Json;
using PartyCli.Domain.Interfaces.Persistence;
using PartyCli.Domain.Models;

namespace PartyCli.Infrastructure.Persistence;

public class JsonLocalServerRepository : ILocalServerRepository
{
    private readonly string _filePath;
    public JsonLocalServerRepository(IFilePathProvider filePathProvider)
    {
        _filePath = filePathProvider.GetServerFilePath();
    }

    public async Task SaveServersAsync(List<Server> servers)
    {
        var json = JsonSerializer.Serialize(servers, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<List<Server>> GetServersAsync()
    {
        if (!File.Exists(_filePath))
            return new List<Server>();

        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<Server>>(json) ?? new List<Server>();
    }

    public Task<bool> HasServersAsync()
    {
        return Task.FromResult(File.Exists(_filePath));
    }
}