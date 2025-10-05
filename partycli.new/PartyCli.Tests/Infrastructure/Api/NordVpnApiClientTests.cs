using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Moq;
using PartyCli.Domain.Models;
using PartyCli.Infrastructure.Api;
using RichardSzalay.MockHttp;
using Shouldly;

namespace PartyCli.Tests.Infrastructure.Api;

public class NordVpnApiClientTests
{
    private const string _baseUrl = "https://api.nordvpn.com";
    private readonly ILogger<NordVpnApiClient> _logger = Mock.Of<ILogger<NordVpnApiClient>>();

    [Fact]
    public async Task GetAllServersListAsync_ReturnsServerList()
    {
        // Arrange
        var expectedServers = new List<Server>
        {
            new("us1234.nordvpn.com", 45, "online"),
            new("us5678.nordvpn.com", 67, "online")
        };

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When($"{_baseUrl}/servers")
            .Respond("application/json", JsonContent.Create(expectedServers).ReadAsStringAsync().Result);

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(_baseUrl);

        var client = new NordVpnApiClient(httpClient, _logger);

        // Act
        var result = await client.GetAllServersListAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result[0].Name.ShouldBe("us1234.nordvpn.com");
        result[0].Load.ShouldBe(45);
        result[1].Name.ShouldBe("us5678.nordvpn.com");
    }

    [Fact]
    public async Task GetAllServersListAsync_WhenApiReturnsNull_ReturnsEmptyList()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When($"{_baseUrl}/servers")
            .Respond("application/json", "null");

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(_baseUrl);

        var client = new NordVpnApiClient(httpClient, _logger);

        // Act
        var result = await client.GetAllServersListAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllServersListAsync_WhenApiReturnsError_ThrowsHttpRequestException()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When($"{_baseUrl}/servers")
            .Respond(HttpStatusCode.InternalServerError);

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(_baseUrl);

        var client = new NordVpnApiClient(httpClient, _logger);

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(() => client.GetAllServersListAsync());
    }

    [Fact]
    public async Task GetServersByTCPListAsync_WithCancellation_ThrowsOperationCanceledException()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When($"{_baseUrl}/servers*")
            .Throw(new OperationCanceledException());

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(_baseUrl);

        var client = new NordVpnApiClient(httpClient, _logger);

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(
            async () => await client.GetServerByProtocolListAsync(Protocols.TCP, cts.Token));
    }
}