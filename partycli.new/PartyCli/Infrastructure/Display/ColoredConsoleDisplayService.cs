using Spectre.Console;
using PartyCli.Domain.Interfaces.Display;
using PartyCli.Domain.Models;

namespace PartyCli.Infrastructure.Display;

public class ColoredConsoleDisplayService : IDisplayService
{
    public void DisplayServers(List<Server> servers)
    {
        AnsiConsole.MarkupLine("[cyan]Server list:[/]");
        AnsiConsole.WriteLine(new string('-', 40));

        foreach (var server in servers)
        {
            AnsiConsole.MarkupLine($"[white]Name:[/]   {server.Name}");
            AnsiConsole.MarkupLine($"[green]Load:[/]   {server.Load}%");
            AnsiConsole.MarkupLine($"[yellow]Status:[/] {server.Status}");
            AnsiConsole.WriteLine(new string('-', 40));
        }

        AnsiConsole.MarkupLine($"[green]Total servers: {servers.Count}[/]");
    }

    public void DisplayError(string message)
    {
        AnsiConsole.MarkupLine($"[red]Error: {Markup.Escape(message)}[/]");
    }

    public void DisplaySuccess(string message)
    {
        AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
    }

    public void DisplayWarning(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(message)}[/]");
    }
}