using MandalayClient.Common.Models.Json.Transactions;
using MandalayJdeIntegrationCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Transformation
{
    public interface IJdeTransformationService
    {
        Task<TransactionBatchResponse> TransformToJdeAsync(TransactionBatchResponse txnBatchResponse);
    }
}
