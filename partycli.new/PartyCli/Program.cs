using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using PartyCli;
using PartyCli.Application.Services;
using PartyCli.Commands;
using PartyCli.Domain.Interfaces.Api;
using PartyCli.Domain.Interfaces.Display;
using PartyCli.Domain.Interfaces.Persistence;
using PartyCli.Infrastructure.Api;
using PartyCli.Infrastructure.Display;
using PartyCli.Infrastructure.Persistence;
using Spectre.Console.Cli;

ConfigureSerilog();

try
{
    Log.Information("Starting PartyCli application");

    var services = new ServiceCollection();

    // Logging
    services.AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.AddSerilog();
    });

    // HTTP Client
    services.AddHttpClient<IVpnServerApiRepository, NordVpnApiClient>();

    // Repositories and Services
    services.AddSingleton<IFilePathProvider, AppDataFilePathProvider>();
    services.AddSingleton<IServerRepository, JsonServerRepository>();
    services.AddSingleton<IDisplayService, ConsoleDisplayService>();
    // services.AddSingleton<IDisplayService, ColoredConsoleDisplayService>();
    services.AddSingleton<IServerService,ServerService>();

    var app = new CommandApp(new TypeRegistrar(services));

    app.Configure(config =>
    {
        config.AddCommand<ServerListCommand>("server_list")
            .WithDescription("Fetch and display VPN servers");
    });

    return await app.RunAsync(args);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}


void ConfigureSerilog()
{
    var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    var logFolder = Path.Combine(appData, "partycli", "logs");
    Directory.CreateDirectory(logFolder);
    var logPath = Path.Combine(logFolder, "partycli-.txt");

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
        .CreateLogger();
}