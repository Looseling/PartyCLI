using PartyCli.Domain.Models;

namespace PartyCli.Domain;

public class ServerFilter
{
    public string? Country { get; set; }
    public Protocols? Protocol { get; set; }

    public bool Local { get; set; }
}
