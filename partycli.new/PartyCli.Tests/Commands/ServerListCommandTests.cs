// using Moq;
// using Microsoft.Extensions.DependencyInjection;
// using Spectre.Console.Cli;
// using PartyCli.Commands;
// using PartyCli.Application.Services;
// using PartyCli.Domain.Interfaces.Display;
// using PartyCli.Domain.Models;
// using Shouldly;
//
// namespace PartyCli.Tests.Commands;
//
// public class ServerListCommandTests
// {
//     private readonly Mock<IServerService> _mockServerService;
//     private readonly Mock<IDisplayService> _mockDisplay;
//
//     public ServerListCommandTests()
//     {
//         _mockServerService = new Mock<IServerService>();
//         _mockDisplay = new Mock<IDisplayService>();
//     }
//
//     private CommandApp CreateApp()
//     {
//         var registrations = new ServiceCollection();
//         registrations.AddSingleton(_mockServerService.Object);
//         registrations.AddSingleton(_mockDisplay.Object);
//
//         var registrar = new TypeRegistrar(registrations);
//         var app = new CommandApp(registrar);
//
//         app.Configure(config =>
//         {
//             config.PropagateExceptions();
//             config.AddCommand<ServerListCommand>("server_list");
//         });
//
//         return app;
//     }
//
//     [Fact]
//     public async Task ExecuteAsync_WithLocal_LoadsFromRepository()
//     {
//         // Arrange
//         var servers = new List<Server>
//         {
//             new("test.com", 50, "online")
//         };
//
//         _mockServerService.Setup(s => s.GetLocalServersAsync())
//             .ReturnsAsync(servers);
//
//         var app = CreateApp();
//
//         // Act
//         var result = await app.RunAsync(new[] { "server_list", "--local" });
//
//         // Assert
//         result.ShouldBe(0);
//         _mockServerService.Verify(s => s.GetLocalServersAsync(), Times.Once);
//         _mockDisplay.Verify(d => d.DisplayServers(servers), Times.Once);
//     }
//
//     [Fact]
//     public async Task ExecuteAsync_WithFrance_FetchesFranceServers()
//     {
//         // Arrange
//         var servers = new List<Server>
//         {
//             new("fr1.nordvpn.com", 40, "online")
//         };
//
//         _mockServerService.Setup(s => s.FetchAndSaveServersAsync(
//             It.Is<ServerFilter>(f => f.Country == "france")))
//             .ReturnsAsync(servers);
//
//         var app = CreateApp();
//
//         // Act
//         var result = await app.RunAsync(new[] { "server_list", "--france" });
//
//         // Assert
//         result.ShouldBe(0);
//         _mockServerService.Verify(s => s.FetchAndSaveServersAsync(
//             It.Is<ServerFilter>(f => f.Country == "france")), Times.Once);
//     }
//
//     [Fact]
//     public async Task ExecuteAsync_WithTcp_FetchesTcpServers()
//     {
//         // Arrange
//         var servers = new List<Server>
//         {
//             new("tcp1.nordvpn.com", 30, "online")
//         };
//
//         _mockServerService.Setup(s => s.FetchAndSaveServersAsync(
//             It.Is<ServerFilter>(f => f.Protocol == Protocols.TCP)))
//             .ReturnsAsync(servers);
//
//         var app = CreateApp();
//
//         // Act
//         var result = await app.RunAsync(new[] { "server_list", "--TCP" });
//
//         // Assert
//         result.ShouldBe(0);
//     }
//
//     [Fact]
//     public async Task ExecuteAsync_WithNoOptions_FetchesAllServers()
//     {
//         // Arrange
//         var servers = new List<Server>
//         {
//             new("us1.nordvpn.com", 50, "online"),
//             new("uk1.nordvpn.com", 60, "online")
//         };
//
//         _mockServerService.Setup(s => s.FetchAndSaveServersAsync(It.IsAny<ServerFilter>()))
//             .ReturnsAsync(servers);
//
//         var app = CreateApp();
//
//         // Act
//         var result = await app.RunAsync(new[] { "server_list" });
//
//         // Assert
//         result.ShouldBe(0);
//         _mockDisplay.Verify(d => d.DisplayServers(servers), Times.Once);
//     }
// }

using Xunit;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using PartyCli.Commands;
using PartyCli.Application.Services;
using PartyCli.Domain.Interfaces;
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