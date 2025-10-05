using PartyCli.Domain.Interfaces.Persistence;

namespace PartyCli.Infrastructure.Persistence;

public class CountryLookupFromDictionary : ICountryLookup
{
    private readonly Dictionary<string, int> _countryMap = new()
    {
        ["france"] = 74,
        ["albania"] = 2,
        ["argentina"] = 10
    };
    public int? GetCountryId(string name) => _countryMap.TryGetValue(name, out var id) ? id : null;
}