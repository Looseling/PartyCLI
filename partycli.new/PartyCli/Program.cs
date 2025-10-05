using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PartyCli.Application.Commands;
using PartyCli.Application.Services;
using PartyCli.Domain.Interfaces.Api;
using PartyCli.Domain.Interfaces.Display;
using PartyCli.Domain.Interfaces.Persistence;
using PartyCli.Infrastructure.Api;
using PartyCli.Infrastructure.Display;
using PartyCli.Infrastructure.Persistence;
using Serilog;
using Spectre.Console.Cli;

try
{
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false, true)
        .AddEnvironmentVariables()
        .Build();

    ConfigureSerilog();

    Log.Information("Starting PartyCli application");

    // Setup DI
    var services = new ServiceCollection();
    ConfigureServices(services, configuration);

    var app = new CommandApp(new MyTypeRegistrar(services));
    ConfigureCommands(app);

    return await app.RunAsync(args);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    Console.WriteLine(ex.Message);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Configuration
    services.AddSingleton(configuration);

    // Logging
    services.AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.AddSerilog();
    });

    // HTTP clients
    services.AddHttpClient<IVpnServerApiClient, NordVpnApiClient>(client =>
    {
        var baseurl = configuration["NordVpn:Server:BaseUrl"] ??
                      throw new NullReferenceException("NordVpn server baseurl is null");
        client.BaseAddress = new Uri(baseurl);
    });


    // Infrastructure
    services.AddSingleton<IFilePathProvider, AppDataFilePathProvider>();
    services.AddSingleton<ILocalServerRepository, JsonLocalServerRepository>();
    services.AddSingleton<ICountryLookup, CountryLookupFromDictionary>();
    // services.AddSingleton<IDisplayService, ConsoleDisplayService>();
    services.AddSingleton<IDisplayService, ColoredConsoleDisplayService>();

    // Application
    services.AddSingleton<IServerService, ServerService>();
}

void ConfigureCommands(CommandApp app)
{
    app.Configure(config =>
    {
        config.ValidateExamples();
        config.PropagateExceptions();
        config.UseStrictParsing();

        config.AddCommand<ServerListCommand>("server_list")
            .WithDescription("Fetch and display VPN servers");
    });
}

void ConfigureSerilog()
{
    var logPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "partycli",
        "logs",
        "partycli-.txt"
    );

    Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

    var loggerConfig = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.File(logPath, rollingInterval: RollingInterval.Day);

#if DEBUG
    loggerConfig.WriteTo.Console();
#endif

    Log.Logger = loggerConfig.CreateLogger();
}