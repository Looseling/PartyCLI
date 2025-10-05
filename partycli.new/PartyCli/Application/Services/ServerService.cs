using Microsoft.Extensions.Logging;
using PartyCli.Domain;
using PartyCli.Domain.Interfaces.Api;
using PartyCli.Domain.Interfaces.Persistence;
using PartyCli.Domain.Models;

namespace PartyCli.Application.Services;

public class ServerService(
    IVpnServerApiClient api,
    ILocalServerRepository localRepo,
    ILogger<ServerService> logger,
    ICountryLookup countryLookup)
    : IServerService
{
    private readonly ILogger<ServerService> _logger = logger;

    public async Task<List<Server>> GetServersAsync(ServerFilter filter)
    {
        List<Server> servers;

        if (filter.Local)
        {
            servers = await localRepo.GetServersAsync();
        }
        else if (!string.IsNullOrEmpty(filter.Country))
        {
            var countryId = countryLookup.GetCountryId(filter.Country);
            if (!countryId.HasValue)
            {
                return new List<Server>();
            }

            servers = await api.GetServerByCountryListAsync(countryId.Value);
        }
        else if (filter.Protocol.HasValue)
        {
            servers = await api.GetServerByProtocolListAsync(filter.Protocol.Value);
        }
        else
        {
            servers = await api.GetAllServersListAsync();
        }

        await localRepo.SaveServersAsync(servers);
        return servers;
    }
}
