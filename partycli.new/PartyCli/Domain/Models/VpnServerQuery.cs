namespace PartyCli.Domain.Models;

public class VpnServerQuery
{
    public int? CountryId { get; set; }
    public int? ProtocolId { get; set; }
    
    public int? TechnologyId { get; set; }
    
    public int? CityId { get; set;}

    public int? RegionId { get; set;}
    
    public int? SpecificServcerId { get; set;}

    public int? ServerGroupId { get; set;}
    
    public VpnServerQuery(int? protocol, int? countryId, int? cityId, int? regionId, int? specificServcerId, int? serverGroupId)
    {
        ProtocolId = protocol;
        CountryId = countryId;
        CityId = cityId;
        RegionId = regionId;
        SpecificServcerId = specificServcerId;
        ServerGroupId = serverGroupId;
    }
}