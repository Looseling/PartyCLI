FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["PartyCli/PartyCli.csproj", "PartyCli/"]
RUN dotnet restore "PartyCli/PartyCli.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/PartyCli"
RUN dotnet build "PartyCli.csproj" -c Release -o /app/build
RUN dotnet publish "PartyCli.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PartyCli.dll"]