using System.Text.Json;
using PartyCli.Domain.Interfaces;

namespace PartyCli.Infrastructure.Persistence;

public class JsonSettingsRepository : ISettingsRepository
{
    private readonly string _filePath;
    private Dictionary<string, string> _cache;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public JsonSettingsRepository()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appData, "partycli");
        Directory.CreateDirectory(appFolder);
        _filePath = Path.Combine(appFolder, "settings.json");
        
        _cache = new Dictionary<string, string>();
        LoadCache();
    }

    public async Task<string?> GetValueAsync(string key)
    {
        await _lock.WaitAsync();
        try
        {
            return _cache.TryGetValue(key, out var value) ? value : null;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task SetValueAsync(string key, string value)
    {
        await _lock.WaitAsync();
        try
        {
            _cache[key] = value;
            await SaveCacheAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> HasValueAsync(string key)
    {
        await _lock.WaitAsync();
        try
        {
            return _cache.ContainsKey(key);
        }
        finally
        {
            _lock.Release();
        }
    }

    private void LoadCache()
    {
        if (!File.Exists(_filePath))
        {
            _cache = new Dictionary<string, string>();
            return;
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            _cache = JsonSerializer.Deserialize<Dictionary<string, string>>(json) 
                     ?? new Dictionary<string, string>();
        }
        catch
        {
            _cache = new Dictionary<string, string>();
        }
    }

    private async Task SaveCacheAsync()
    {
        var json = JsonSerializer.Serialize(_cache, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        await File.WriteAllTextAsync(_filePath, json);
    }
}