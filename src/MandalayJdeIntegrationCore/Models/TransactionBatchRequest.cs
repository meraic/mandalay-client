namespace MandalayJdeIntegrationCore.Models
{
    public class TransactionBatchRequest
    {
        public string Site { get; set; }
        public string Tenant { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
