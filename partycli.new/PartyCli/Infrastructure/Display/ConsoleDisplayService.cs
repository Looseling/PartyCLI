using System;
using System.Collections.Generic;
using PartyCli.Domain.Interfaces;
using PartyCli.Domain.Interfaces.Display;
using PartyCli.Domain.Models;

namespace PartyCli.Infrastructure.Display;

public class ConsoleDisplayService : IDisplayService
{
    public void DisplayServers(List<Server> servers)
    {
        Console.WriteLine("Server list:");
        Console.WriteLine("─────────────────────────────────────");
        foreach (var server in servers)
        {
            Console.WriteLine($"Name:   {server.Name}");
            Console.WriteLine($"Load:   {server.Load}%");
            Console.WriteLine($"Status: {server.Status}");
            Console.WriteLine("─────────────────────────────────────");
        }
        Console.WriteLine("Total servers: " + servers.Count);
    }

    public void DisplayError(string message)
    {
        Console.WriteLine(message);
    }

    public void DisplaySuccess(string message)
    {
        Console.WriteLine(message);
    }

    public void DisplayWarning(string message)
    {
        Console.WriteLine(message);
    }
}
