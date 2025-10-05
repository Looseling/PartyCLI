# PartyCli

Modernized CLI (partycli.new) and legacy app (partycli.old). This guide explains what’s what and how to run each.

## Repository layout
- partycli.new/ — .NET 9 solution (modernized). Projects:
  - PartyCli/ — main CLI app
  - PartyCli.Tests/ — unit tests
- partycli.old/ — original .NET Framework 4.8 console app for reference

## Prerequisites
- .NET SDK 9.x (to run the new app)
- Docker Desktop (optional, for containerized runs)

## Run the new app (.NET 9)
- Build:
  - `dotnet build partycli.new/PartyCli/PartyCli.csproj -c Release`
- Run (examples):
  - All servers: `dotnet run --project partycli.new/PartyCli -- server_list`
  - TCP servers: `dotnet run --project partycli.new/PartyCli -- server_list --TCP`
  - By country: `dotnet run --project partycli.new/PartyCli -- server_list --country france`
  - Local cache only: `dotnet run --project partycli.new/PartyCli -- server_list --local`

Flags supported:
- `--local` display servers from local cache only
- `--TCP` fetch servers that support the TCP protocol
- `--country <name>` fetch by country (e.g., france, albania, argentina)

## Docker (new app)
This repo includes a multi-stage Dockerfile under `partycli.new/`. It builds a small runtime image (Linux by default).

```
cd partycli.new

docker build -t partycli:latest .

# Inline run
docker run --rm partycli:latest server_list

# With flags
docker run --rm partycli:latest server_list --TCP
```

## Tests (new app)
Frameworks: xUnit, Shouldly, Moq, Spectre.Console.Testing

- Run tests: `dotnet test partycli.new/PartyCli.Tests/PartyCli.Tests.csproj -c Release`

## Configuration and data (new app)
- API base URL: `partycli.new/PartyCli/appsettings.json`
- Local cache file: `%APPDATA%/partycli/servers.json`
- Logs (Serilog): `%APPDATA%/partycli/logs/partycli-*.txt`

## Run the legacy app (.NET Framework 4.8)
The original app is kept under `partycli.old/` for comparison.

Options:
- Open `partycli.old/partycli.sln` in Visual Studio (with .NET Framework 4.8 developer pack) and run.
- Or run the existing binary (if present): `partycli.old/partycli/bin/Debug/partycli.exe`

Examples (legacy CLI):
- `partycli.exe server_list`
- `partycli.exe server_list --france` (legacy flag)
- `partycli.exe server_list --TCP`
- `partycli.exe server_list --local`

Note: In the modern app, `--france` is generalized to `--country france`. 
This is done to avoid maintaining country options in Command layer and to pull from available data source 

## What’s improved in the new app
- Spectre.Console.Cli command model with DI (easy to add commands/options)
- Clear abstractions: API client, persistence, display (easy to swap)
- Typed HttpClient + configuration
- Serilog logging to file/console
- Unit tests for command, API client, and persistence
- Dockerized runtime image

## Troubleshooting
- Empty output with `--local`: no cache yet. Run without `--local` once to populate.
- HTTP errors: check your network and the API base URL in `appsettings.json`.
- File/permission errors: ensure `%APPDATA%/partycli` is writable (created automatically).
- Docker on Windows: ensure Linux containers are enabled if the image fails to start.
