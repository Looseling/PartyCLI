using System.Collections.Generic;
using PartyCli.Domain.Models;

namespace PartyCli.Domain.Interfaces.Display;

public interface IDisplayService
{
    void DisplayServers(List<Server> servers);
    void DisplayError(string message);
    void DisplaySuccess(string message);
    void DisplayWarning(string message);
}
