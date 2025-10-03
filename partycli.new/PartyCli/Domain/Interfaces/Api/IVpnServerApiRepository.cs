using PartyCli.Domain.Models;

namespace PartyCli.Domain.Interfaces.Api;

public interface IVpnServerApiRepository
{
    Task<List<Server>> GetServerByProtocolListAsync(Protocols vpnProtocol, CancellationToken cancellationToken = default);
    Task<List<Server>> GetAllServersListAsync(CancellationToken cancellationToken = default);
    Task<List<Server>> GetServerByCountryListAsync(int countryId, CancellationToken cancellationToken = default);
}