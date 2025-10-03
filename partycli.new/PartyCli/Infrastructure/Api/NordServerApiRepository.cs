using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PartyCli.Domain.Models;
using PartyCli.Domain.Interfaces.Api;

namespace PartyCli.Infrastructure.Api;

public class NordVpnApiClient : IVpnServerApiRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NordVpnApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public NordVpnApiClient(HttpClient httpClient, ILogger<NordVpnApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<Server>> GetServerByProtocolListAsync(Protocols vpnProtocol, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching servers for protocol {Protocol}", vpnProtocol);
     
        var vpnProtocolId = (int)vpnProtocol;
        
        var url = $"{Constants.BaseNordUrl}servers?filters[servers_technologies][id]={(vpnProtocolId)}";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var servers = await response.Content.ReadFromJsonAsync<List<Server>>(_jsonOptions, cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} servers for protocol {Protocol}", servers?.Count ?? 0, vpnProtocol);
        return servers ?? new List<Server>();
    }

    public async Task<List<Server>> GetAllServersListAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching all servers");
        
        var url = $"{Constants.BaseNordUrl}servers";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var servers = await response.Content.ReadFromJsonAsync<List<Server>>(_jsonOptions, cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} servers", servers?.Count ?? 0);
        return servers ?? new List<Server>();
    }

    public async Task<List<Server>> GetServerByCountryListAsync(int countryId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching servers for country {CountryId}", countryId);
        
        var url = $"{Constants.BaseNordUrl}servers?filters[servers_technologies][id]={Protocols.NordLynx}&filters[country_id]={countryId}";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var servers = await response.Content.ReadFromJsonAsync<List<Server>>(_jsonOptions, cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} servers for country {CountryId}", servers?.Count ?? 0, countryId);
        return servers ?? new List<Server>();
    }
}