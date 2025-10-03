using Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using PartyCli.Commands;
using PartyCli.Application.Services;
using PartyCli.Domain.Interfaces.Display;
using PartyCli.Domain.Models;
using Shouldly;

namespace PartyCli.Tests.Commands;

public class ServerListCommandTests
{
    private readonly Mock<IServerService> _mockServerService;
    private readonly Mock<IDisplayService> _mockDisplay;
    private readonly Mock<ILogger<ServerListCommand>> _mockLogger;

    public ServerListCommandTests()
    {
        _mockServerService = new Mock<IServerService>();
        _mockDisplay = new Mock<IDisplayService>();
        _mockLogger = new Mock<ILogger<ServerListCommand>>();
    }

    private CommandApp CreateApp()
    {
        var services = new ServiceCollection();
        
        // Register the mocks
        services.AddSingleton(_mockServerService.Object);
        services.AddSingleton(_mockDisplay.Object);
        services.AddSingleton(_mockLogger.Object);

        var registrar = new TypeRegistrar(services);
        var app = new CommandApp(registrar);

        app.Configure(config =>
        {
            config.PropagateExceptions();
            config.AddCommand<ServerListCommand>("server_list");
        });

        return app;
    }

    [Fact]
    public async Task ExecuteAsync_WithLocal_LoadsFromRepository()
    {
        // Arrange
        var servers = new List<Server>
        {
            new("test.com", 50, "online")
        };

        _mockServerService.Setup(s => s.GetLocalServersAsync())
            .ReturnsAsync(servers);

        var app = CreateApp();

        // Act
        var result = await app.RunAsync(new[] { "server_list", "--local" });

        // Assert
        result.ShouldBe(0);
        _mockServerService.Verify(s => s.GetLocalServersAsync(), Times.Once);
        _mockDisplay.Verify(d => d.DisplayServers(servers), Times.Once);
    }
}