using MandalayJdeIntegrationCore.Models;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Load
{
    public interface ILoadToJdeService
    {
        Task<TransactionBatchResponse> SendTransactionsToJdeAsync(TransactionBatchResponse txnBatchResponse);
    }
}