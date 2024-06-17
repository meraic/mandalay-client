using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Discard
{
    public interface IDiscardBatchService
    {
        Task DiscardBatchAsync(string batchId, string tenantId);
    }
}
