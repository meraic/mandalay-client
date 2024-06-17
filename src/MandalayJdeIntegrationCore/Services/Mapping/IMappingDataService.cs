using MandalayJdeIntegrationCore.Models.Mapping;
using System.Collections.Generic;

namespace MandalayJdeIntegrationCore.Services.Mapping
{
    public interface IMappingDataService
    {
        IDictionary<string, Site> SiteMappings { get; set; }
        IDictionary<string, Tenant> TenantMappings { get; set; }
        IDictionary<string, UnitOfMeasure> UomMappings { get; set; }
    }
}
