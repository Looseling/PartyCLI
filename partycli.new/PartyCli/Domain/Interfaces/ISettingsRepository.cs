namespace PartyCli.Domain.Interfaces;

public interface ISettingsRepository
{
    Task<string?> GetValueAsync(string key);
    
    Task SetValueAsync(string key, string value);

    Task<bool> HasValueAsync(string key);
}