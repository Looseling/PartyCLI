namespace PartyCli.Domain.Models;


/// <summary>
/// Represents a VPN server from the NordVPN API
/// </summary>
public record Server(
    string Name,
    int Load,
    string Status
);