# PartyCli

A small, human-friendly command-line app that fetches and shows VPN servers. You can pull fresh data from NordVPN public endpoints, filter it, and also read/write a local cache on your machine.

## Quick start
- Build and run locally (requires .NET 9 SDK):
  - `dotnet build PartyCli/PartyCli.csproj -c Release`
  - `dotnet run --project PartyCli -- server_list [--local] [--TCP] [--country <name>]`

Examples:
- Fetch all servers and save locally: `dotnet run --project PartyCli -- server_list`
- Fetch TCP servers: `dotnet run --project PartyCli -- server_list --TCP`
- Fetch by country (e.g., france): `dotnet run --project PartyCli -- server_list --country france`
- Show locally cached servers: `dotnet run --project PartyCli -- server_list --local`

## Docker
This repo includes a multi-stage Dockerfile that builds a small runtime image. by default it builds linux container.

  ```
    // build the image
    docker build -t partycli:latest .
    
    // inline run
    docker run --rm partycli:latest server_list
    
    //inline run with flags
    docker run --rm partycli:latest server_list --TCP
  ```
## Testing
Frameworks used in tests:
- xUnit (test framework)
- Shouldly (assertions)
- Moq (mocking)
- Spectre.Console.Testing (console interactions)
- coverlet.collector (code coverage)

Run tests:
- `dotnet test PartyCli.Tests/PartyCli.Tests.csproj -c Release`

## Architecture (high level)
- `Program.cs` wires up the app using dependency injection and Serilog for logging.
- Command layer (Spectre.Console.Cli):
  - `server_list` command with flags: `--local`, `--TCP`, `--country <name>`.
- Application services:
  - `ServerService` orchestrates fetching from API or reading local cache, then saving results.
- Infrastructure:
  - API client: `NordVpnApiClient` uses `HttpClient` to call NordVPN endpoints.
  - Persistence: `JsonServerRepository` saves/loads servers as JSON under app data via `AppDataFilePathProvider`.
  - Display: `ConsoleDisplayService` prints results to the terminal.
- Domain:
  - Interfaces and models (`Server`, `Protocols`, etc.) used across layers.

## Useful flags
- `--local`: display servers from local cache only.
- `--TCP`: fetch servers supporting TCP protocol.
- `--country <name>`: fetch by country (e.g., france, albania, argentina).
