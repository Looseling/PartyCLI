using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PartyCli.Application.Commands;
using PartyCli.Application.Services;
using PartyCli.Domain;
using PartyCli.Domain.Interfaces.Display;
using PartyCli.Domain.Models;
using Shouldly;
using Spectre.Console.Testing;

namespace PartyCli.Tests.Commands;

public class ServerListCommandTests
{
    private readonly TestConsole _console = new();
    private readonly Mock<IDisplayService> _display = new();
    private readonly Mock<ILogger<ServerListCommand>> _logger = new();
    private readonly Mock<IServerService> _serverService = new();

    [Fact]
    public async Task ExecuteAsyncTest_WithCommandAppTester_UsesMocks()
    {
        // Arrange
        var serverServiceMock = new Mock<IServerService>();
        var displayMock = new Mock<IDisplayService>();
        var loggerMock = new Mock<ILogger<ServerListCommand>>();

        serverServiceMock.Setup(s => s.GetServersAsync(It.IsAny<ServerFilter>()))
            .ReturnsAsync(new List<Server> { new("us1234.nordvpn.com", 42, "online") });

        var services = new ServiceCollection();
        services.AddSingleton(serverServiceMock.Object);
        services.AddSingleton(displayMock.Object);
        services.AddSingleton(loggerMock.Object);

        var registrar = new MyTypeRegistrar(services);

        var app = new CommandAppTester(registrar);
        app.SetDefaultCommand<ServerListCommand>();

        // Act
        var result = await app.RunAsync("--server_list");

        // Assert
        result.ExitCode.ShouldBe(0);
        displayMock.Verify(d => d.DisplayServers(It.IsAny<List<Server>>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenServersExist_DisplaysServers()
    {
        // Arrange
        var servers = new List<Server> { new("us1234.nordvpn.com", 42, "online") };

        var serverServiceMock = new Mock<IServerService>();
        serverServiceMock.Setup(s => s.GetServersAsync(It.IsAny<ServerFilter>()))
            .ReturnsAsync(servers);

        var displayMock = new Mock<IDisplayService>();
        var loggerMock = new Mock<ILogger<ServerListCommand>>();

        var services = new ServiceCollection();
        services.AddSingleton(serverServiceMock.Object);
        services.AddSingleton(displayMock.Object);
        services.AddSingleton(loggerMock.Object);

        var app = new CommandAppTester(new MyTypeRegistrar(services));
        app.SetDefaultCommand<ServerListCommand>();

        // Act
        var result = await app.RunAsync("--server_list");

        // Assert
        result.ExitCode.ShouldBe(0);
        displayMock.Verify(d => d.DisplayServers(servers), Times.Once);
        displayMock.Verify(d => d.DisplayWarning(It.IsAny<string>()), Times.Never);
        displayMock.Verify(d => d.DisplayError(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoServers_DisplaysWarning()
    {
        // Arrange
        var serverServiceMock = new Mock<IServerService>();
        serverServiceMock.Setup(s => s.GetServersAsync(It.IsAny<ServerFilter>()))
            .ReturnsAsync(new List<Server>());

        var displayMock = new Mock<IDisplayService>();
        var loggerMock = new Mock<ILogger<ServerListCommand>>();

        var services = new ServiceCollection();
        services.AddSingleton(serverServiceMock.Object);
        services.AddSingleton(displayMock.Object);
        services.AddSingleton(loggerMock.Object);

        var app = new CommandAppTester(new MyTypeRegistrar(services));
        app.SetDefaultCommand<ServerListCommand>();

        // Act
        var result = await app.RunAsync("--server_list", "--local");

        // Assert
        result.ExitCode.ShouldBe(0);
        displayMock.Verify(d => d.DisplayWarning("No local servers found"), Times.Once);
        displayMock.Verify(d => d.DisplayServers(It.IsAny<List<Server>>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExceptionThrown_DisplaysErrorAndReturns1()
    {
        // Arrange
        var serverServiceMock = new Mock<IServerService>();
        serverServiceMock.Setup(s => s.GetServersAsync(It.IsAny<ServerFilter>()))
            .ThrowsAsync(new InvalidOperationException("Boom"));

        var displayMock = new Mock<IDisplayService>();
        var loggerMock = new Mock<ILogger<ServerListCommand>>();

        var services = new ServiceCollection();
        services.AddSingleton(serverServiceMock.Object);
        services.AddSingleton(displayMock.Object);
        services.AddSingleton(loggerMock.Object);

        var app = new CommandAppTester(new MyTypeRegistrar(services));
        app.SetDefaultCommand<ServerListCommand>();

        // Act
        var result = await app.RunAsync("--server_list");

        // Assert
        result.ExitCode.ShouldBe(1);
        displayMock.Verify(d => d.DisplayError(It.Is<string>(s => s.Contains("Boom"))), Times.Once);
    }
}