namespace MandalayJdeIntegrationCore.Models.Mapping
{
    public class UnitOfMeasure
    {
        public string PartitionKey { get; set; } = "Uom";
        public string MandalayStockUnit { get; set; }
        public string JdeUnitOfMeasure { get; set; }
    }
}
