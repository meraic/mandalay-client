namespace MandalayJdeIntegrationCore.Models.Mapping
{
    public class Site
    {
        public string PartitionKey { get; set; } = "SiteMapping";
        public string Code { get; set; }
        public string Name { get; set; }
        public string MandalaySiteId { get; set; }
        public string MandalayTenantId { get; set; }
        public string JdeBusinessUnit { get; set; }
        public string JdeCompanyNumber { get; set; }
        public bool JdeSplitEpaLevy { get; set; }
        public string JdeEpaLevyItemNumber { get; set; }
    }
}
