using MandalayJdeIntegrationCore.Models;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Blob
{
    public interface IBlobDataService
    {
        Task ExportToBlobContainerAsync(string data, string blobName, BlobContainer blobContainer);
    }
}
