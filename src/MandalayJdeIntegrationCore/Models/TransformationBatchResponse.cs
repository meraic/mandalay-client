using MandalayClient.Common.Models.Json.Transactions;
using System.Collections.Generic;

namespace MandalayJdeIntegrationCore.Models
{
    public class TransformationBatchResponse
    {
        public string BatchId { get; set; }
        public BatchStatus BatchStatus { get; set; }
        public string TenantId { get; set; }
        public string SiteId { get; set; }
        public IEnumerable<JdeTransaction> JdeTransactions { get; set; }
    }
}
