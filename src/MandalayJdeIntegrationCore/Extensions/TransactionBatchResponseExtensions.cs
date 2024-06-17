using MandalayJdeIntegrationCore.Models;
using System.Linq;

namespace MandalayJdeIntegrationCore.Extensions
{
    public static class TransactionBatchResponseExtensions
    {
        public static dynamic ToResponse(this TransactionBatchResponse txnBatchResponse, TransactionBatchRequest txnBatchRequest = null, string status = "Success")
        {
            return new
            {
                Tenant = txnBatchRequest?.Tenant,
                Site = txnBatchRequest?.Site,
                TenantId = txnBatchResponse?.TenantId,
                BatchId = txnBatchResponse?.BatchId,
                SiteId = txnBatchResponse?.SiteId,
                MandalayTransactionsCount = txnBatchResponse?.Transactions?.Count() ?? null,
                Status = status,
                Errors = txnBatchResponse.Errors,
            };
        }
    }
}
