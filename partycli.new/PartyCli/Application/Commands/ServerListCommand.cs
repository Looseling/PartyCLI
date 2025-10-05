using System.ComponentModel;
using Microsoft.Extensions.Logging;
using PartyCli.Application.Services;
using PartyCli.Domain;
using PartyCli.Domain.Interfaces.Display;
using PartyCli.Domain.Models;
using Spectre.Console.Cli;

namespace PartyCli.Application.Commands;

public class ServerListSettings : CommandSettings
{
    [CommandOption("--local")]
    [Description("Display servers from local storage")]
    public bool Local { get; set; }

    [CommandOption("--TCP")]
    [Description("Fetch TCP protocol servers")]
    public bool Tcp { get; set; }

    [CommandOption("--country <COUNTRY>")]
    [Description("Fetch servers for specific country (e.g., france, albania, argentina)")]
    public string? Country { get; set; }
}

public class ServerListCommand : AsyncCommand<ServerListSettings>
{
    private readonly IServerService _serverService;
    private readonly IDisplayService _display;
    private readonly ILogger<ServerListCommand> _logger;

    public ServerListCommand(IServerService serverService,
        IDisplayService display,
        ILogger<ServerListCommand> logger)
    {
        _serverService = serverService;
        _display = display;
        _logger = logger;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ServerListSettings settings)
    {
        try
        {
            var filter = new ServerFilter
            {
                Country = settings.Country,
                Protocol = settings.Tcp ? Protocols.TCP : null,
                Local = settings.Local
            };
            var servers = await _serverService.GetServersAsync(filter);

            if (!servers.Any())
            {
                _display.DisplayWarning(settings.Local
                    ? "No local servers found"
                    : "No servers found matching criteria");
                return 0;
            }

            _display.DisplayServers(servers);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing server_list command");
            _display.DisplayError($"Failed to fetch servers: {ex.Message}");
            return 1;
        }
    }
}