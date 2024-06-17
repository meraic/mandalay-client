using MandalayClient.Common.Models.Json.Transactions;
using System;

namespace MandalayJdeIntegrationCore.Models
{
    public class NausTransactionBatchRequest : CreateBatchRequest 
    {
        public DateTime TenancyLocalTime { get; set; }
    }
}
