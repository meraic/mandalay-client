using MandalayJdeIntegrationCore.Models.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableStorage.Abstractions.POCO;

namespace MandalayJdeIntegrationCore.Services.Mapping
{
    public class MappingDataService : IMappingDataService
    {
        private readonly string tableConnection;
        private readonly PocoTableStore<Tenant, string, string> tenantTableStore;
        private readonly PocoTableStore<Site, string, string> siteTableStore;
        private readonly PocoTableStore<UnitOfMeasure, string, string> uomTableStore;

        public MappingDataService()
        {
            tableConnection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            tenantTableStore = new PocoTableStore<Tenant, string, string>(nameof(Tenant), tableConnection, tenant => tenant.PartitionKey, tenant => tenant.Code);
            siteTableStore = new PocoTableStore<Site, string, string>(nameof(Site), tableConnection, site => site.PartitionKey, site => site.Code);
            uomTableStore = new PocoTableStore<UnitOfMeasure, string, string>(nameof(UnitOfMeasure), tableConnection, uom => uom.PartitionKey, uom => uom.MandalayStockUnit);
            LoadAllMappings().Wait();
        }

        public async Task LoadAllMappings()
        {
            var siteMappings = await siteTableStore.GetAllRecordsAsync().ConfigureAwait(false);
            var tenantMappings = await tenantTableStore.GetAllRecordsAsync().ConfigureAwait(false);
            var uomMappings = await uomTableStore.GetAllRecordsAsync().ConfigureAwait(false);

            SiteMappings = siteMappings.ToList().ToDictionary(site => site.Code);
            TenantMappings = tenantMappings.ToList().ToDictionary(tenant => tenant.Code);
            UomMappings = uomMappings.ToList().ToDictionary(uom => uom.MandalayStockUnit);
        }

        public IDictionary<string, Site> SiteMappings { get; set; }

        public IDictionary<string, Tenant> TenantMappings { get; set; }

        public IDictionary<string, UnitOfMeasure> UomMappings { get; set; }
    }
}
