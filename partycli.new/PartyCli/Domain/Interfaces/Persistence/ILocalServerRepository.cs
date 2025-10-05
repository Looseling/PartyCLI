using PartyCli.Domain.Models;

namespace PartyCli.Domain.Interfaces.Persistence;

public interface ILocalServerRepository
{
    Task SaveServersAsync(List<Server> servers);
    Task<List<Server>> GetServersAsync();
}