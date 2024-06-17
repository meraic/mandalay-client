using MandalayClient;
using MandalayClient.Common.Models.Json.Transactions;
using System;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Discard
{
    public class DiscardBatchService : IDiscardBatchService
    {
        private readonly INausTransactionExportApiClient nausTxnExportApiClient;

        public DiscardBatchService(INausTransactionExportApiClient nausTxnExportApiClient)
        {
            this.nausTxnExportApiClient = nausTxnExportApiClient;
        }

        public async Task DiscardBatchAsync(string batchId, string tenantId)
        {
            if (string.IsNullOrEmpty(batchId)) throw new ArgumentNullException("batchId");
            if (string.IsNullOrEmpty(tenantId)) throw new ArgumentNullException("tenantId");

            await nausTxnExportApiClient.DiscardBatchAsync(new DiscardBatchRequest
            {
                BatchId = batchId,
                TenantId = tenantId
            });
        }
    }
}
