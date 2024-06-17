using MandalayClient.Common.Models.Json.Transactions;
using MandalayJdeIntegrationCore.Models.Mapping;
using System;
using System.Collections.Generic;

namespace MandalayJdeIntegrationCore.Models
{
    public class TransactionBatchResponse
    {
        public TransactionBatchResponse()
        {
            Errors = new List<string>();
        }

        public string Tenant { get; set; }
        public string Site { get; set; }
        public string BatchId { get; set; }
        public BatchStatus BatchStatus { get; set; }
        public string TenantId { get; set; }
        public string SiteId { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
        public IEnumerable<JdeTransaction> JdeTransactions { get; set; }
        public string JdeTransactionsCsv { get; set; }
        public bool HasErrors { get; set; }
        public List<string> Errors;
        public bool IsBadRequest { get; set; }
        public IDictionary<string, Site> SiteMappings { get; set; }
        public IDictionary<string, Site> TenantMappings { get; set; }
        public IDictionary<string, Site> UomMappings { get; set; }
        public string PeriodStartDate { get; set; }
        public string PeriodEndDate { get; set; }
        public DateTime TenantLocalDateTime { get; set; }
    }
}
