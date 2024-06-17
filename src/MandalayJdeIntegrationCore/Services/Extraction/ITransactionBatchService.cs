using MandalayClient.Common.Models.Json.Transactions;
using MandalayJdeIntegrationCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Extraction
{
    public interface ITransactionBatchService
    {
        Task<TransactionBatchResponse> GetTransactionBatchAsync(TransactionBatchRequest request);
        Task<bool> IsBatchCreatedWithTransactionsAsync(string tenantId, string batchId);
        Task<bool> IsBatchCreatedByPagedBatchesAsync(string tenantId, string batchId);
        Task<IEnumerable<Transaction>> GetCreatedBatchTransactionsAsync(string tenantId, string batchId);
        Task DiscardBatchTransactionsAsync(string batchId, string tenantId);
    }
}
