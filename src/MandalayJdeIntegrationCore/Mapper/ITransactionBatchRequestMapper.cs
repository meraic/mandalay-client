using MandalayJdeIntegrationCore.Models;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Mapper
{
    public interface ITransactionBatchRequestMapper
    {
        Task<NausTransactionBatchRequest> MapFrom(TransactionBatchRequest request);
    }
}
