using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using PartyCli.Domain.Interfaces.Persistence;
using PartyCli.Domain.Models;

namespace PartyCli.Infrastructure.Persistence;

public class JsonServerRepository : IServerRepository
{
    private readonly string _filePath;
    public JsonServerRepository(IFilePathProvider filePathProvider)
    {
        _filePath = filePathProvider.GetServerFilePath();
    }
    
    // Tests should be able to mock the filePath
    internal JsonServerRepository(string filePath)
    {
        _filePath = filePath;
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
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