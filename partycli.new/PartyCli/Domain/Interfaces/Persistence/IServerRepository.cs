using System.Collections.Generic;
using System.Threading.Tasks;
using PartyCli.Domain.Models;

namespace PartyCli.Domain.Interfaces.Persistence;

public interface IServerRepository
{
    Task SaveServersAsync(List<Server> servers);
    Task<List<Server>> GetServersAsync();
}