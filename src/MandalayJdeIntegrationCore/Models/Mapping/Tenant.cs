namespace MandalayJdeIntegrationCore.Models.Mapping
{
    public class Tenant
    {
        public string PartitionKey { get; set; } = "TenantMapping";
        public string Code { get; set; }
        public string Name { get; set; }
        public string MandalayTenantId { get; set; }
        public string TimeZone { get; set; }
    }
}
