using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PartyCli.Domain.Interfaces.Api;
using PartyCli.Domain.Interfaces.Persistence;
using PartyCli.Domain.Models;

namespace PartyCli.Application.Services;

public class ServerService : IServerService
{
    private readonly IVpnServerApiRepository _api;
    private readonly IServerRepository _serverRepository;
    private readonly ILogger<ServerService> _logger;

    public ServerService(
        IVpnServerApiRepository api,
        IServerRepository serverRepository,
        ILogger<ServerService> logger)
    {
        _api = api;
        _serverRepository = serverRepository;
        _logger = logger;
    }

    public async Task<List<Server>> GetLocalServersAsync()
    {
        _logger.LogDebug("Loading servers from local storage");
        return await _serverRepository.GetServersAsync();
    }

    public async Task<List<Server>> FetchAndSaveServersAsync(ServerFilter filter)
    {
        List<Server> servers;

        if (!string.IsNullOrEmpty(filter.Country))
        {
            var countryId = GetCountryIdByNameAsync(filter.Country);
            if (!countryId.HasValue)
            {
                _logger.LogWarning("Invalid country: {Country}", filter.Country);
                return new List<Server>();
            }
            
            _logger.LogDebug("Fetching servers for country {Country} (ID: {CountryId})", filter.Country, countryId);
            servers = await _api.GetServerByCountryListAsync(countryId.Value);
        }
        else if (filter.Protocol.HasValue)
        {
            _logger.LogDebug("Fetching servers for protocol {Protocol}", filter.Protocol);
            servers = await _api.GetServerByProtocolListAsync(filter.Protocol.Value);
        }
        else
        {
            _logger.LogDebug("Fetching all servers");
            servers = await _api.GetAllServersListAsync();
        }

        await _serverRepository.SaveServersAsync(servers);
        return servers;
    }
    
    public int? GetCountryIdByNameAsync(string countryName)
    {
        _logger.LogDebug("Looking up country ID for {Country}", countryName);
        
        var countryMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["france"] = 74,
            ["albania"] = 2,
            ["argentina"] = 10,
        };

        if (countryMap.TryGetValue(countryName, out var id))
        {
            return id;
        }

        _logger.LogWarning("Unknown country: {Country}", countryName);
        return null;
    }
}

public class ServerFilter
{
    public string? Country { get; set; }
    public Protocols? Protocol { get; set; }

    public static ServerFilter ForCountry(string country) => new() { Country = country };
    public static ServerFilter ForProtocol(Protocols protocol) => new() { Protocol = protocol };
    public static ServerFilter All() => new();
}

public interface IServerService
{
    Task<List<Server>> GetLocalServersAsync();
    Task<List<Server>> FetchAndSaveServersAsync(ServerFilter filter);
    int? GetCountryIdByNameAsync(string countryName);
}