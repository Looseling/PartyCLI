using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PartyCli.Application.Services;
using PartyCli.Domain.Interfaces.Display;
using PartyCli.Domain.Models;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace PartyCli.Commands;

public class ServerListSettings : CommandSettings
{
    [CommandOption("--local")]
    [Description("Display servers from local storage")]
    public bool Local { get; set; }
    
    [CommandOption("--TCP")]
    [Description("Fetch TCP protocol servers")]
    public bool Tcp { get; set; }

    [CommandOption("--france")]
    [Description("Fetch France servers")]
    public bool France { get; set; }

    [CommandOption("--country <COUNTRY>")]
    [Description("Fetch servers for specific country (e.g., france, albania, argentina)")]
    public string? Country { get; set; }
}

public class ServerListCommand : AsyncCommand<ServerListSettings>
{
    private readonly IServerService _serverService;
    private readonly IDisplayService _display;
    private readonly ILogger<ServerListCommand> _logger;

    public ServerListCommand(
        IServerService serverService,
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
            List<Server> servers;

            if (settings.Local)
            {
                servers = await _serverService.GetLocalServersAsync();
                
                if (!servers.Any())
                {
                    _display.DisplayError("No local servers found");
                    return 0;
                }
            }
            else
            {
                var filter = BuildFilterAsync(settings);
                servers = await _serverService.FetchAndSaveServersAsync(filter);
                
                if (!servers.Any())
                {
                    _display.DisplayWarning("No servers found matching criteria");
                    return 0;
                }
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

    private ServerFilter BuildFilterAsync(ServerListSettings settings)
    {
        if (!string.IsNullOrEmpty(settings.Country))
        {
            return ServerFilter.ForCountry(settings.Country);
        }
        if (settings.Tcp)
        {
            return ServerFilter.ForProtocol(Protocols.TCP);
        }

        return ServerFilter.All();
    }
}