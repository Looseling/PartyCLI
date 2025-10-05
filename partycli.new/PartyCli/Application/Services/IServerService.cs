using PartyCli.Domain;
using PartyCli.Domain.Models;

namespace PartyCli.Application.Services;

public interface IServerService
{
    Task<List<Server>> GetServersAsync(ServerFilter filter);
}