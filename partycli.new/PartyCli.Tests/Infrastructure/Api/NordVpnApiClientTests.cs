using System.Net;
using System.Net.Http.Json;
using Moq;
using Moq.Protected;
using Shouldly;
using Microsoft.Extensions.Logging;
using PartyCli.Infrastructure.Api;
using PartyCli.Domain.Models;

namespace PartyCli.Tests.Infrastructure.Api;

public class NordVpnApiClientTests
{
    private readonly Mock<ILogger<NordVpnApiClient>> _mockLogger = new();

    [Fact]
    public async Task GetAllServersListAsync_ReturnsServerList()
    {
        // Arrange
        var expectedServers = new List<Server>
        {
            new("us1234.nordvpn.com", 45, "online") ,
            new("us5678.nordvpn.com", 67, "online")
        };

        var httpClient = CreateMockHttpClient(expectedServers, HttpStatusCode.OK);
        var client = new NordVpnApiClient(httpClient, _mockLogger.Object);

        // Act
        var result = await client.GetAllServersListAsync();

        // Assert
        result.Count.ShouldBe(2);
        result[0].Name.ShouldBe("us1234.nordvpn.com");
        result[0].Load.ShouldBe(45);
        result[1].Name.ShouldBe("us5678.nordvpn.com");
    }

    [Fact]
    public async Task GetAllServersListAsync_WhenApiReturnsNull_ReturnsEmptyList()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create<List<Server>>(null)
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var client = new NordVpnApiClient(httpClient, _mockLogger.Object);

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
        var httpClient = CreateMockHttpClient(new List<Server>(), HttpStatusCode.InternalServerError);
        var client = new NordVpnApiClient(httpClient, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            async () => await client.GetAllServersListAsync());
    }

    [Fact]
    public async Task GetServersByTCPListAsync_WithCancellation_ThrowsOperationCanceledException()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        var httpClient = new HttpClient(mockHandler.Object);
        var client = new NordVpnApiClient(httpClient, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await client.GetServerByProtocolListAsync(Protocols.TCP, cts.Token));
    }

    private HttpClient CreateMockHttpClient(List<Server> servers, HttpStatusCode statusCode)
    {
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = JsonContent.Create(servers)
            });

        return new HttpClient(mockHandler.Object);
    }
}