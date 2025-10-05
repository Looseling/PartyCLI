namespace PartyCli.Domain.Interfaces.Persistence;

public interface ICountryLookup
{
    int? GetCountryId(string name);
}