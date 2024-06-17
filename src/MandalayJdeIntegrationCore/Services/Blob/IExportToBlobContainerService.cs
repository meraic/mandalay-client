using MandalayJdeIntegrationCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Blob
{
    public interface IExportToBlobContainerService
    {
        Task ExportTransactionsToBlobContainerAsync<T>(IEnumerable<T> data, BlobContainer blobContainer, BlobType blobType, 
            string blobNameSuffix = null, string blobNamePrefix = null);

        Task ExportTransactionsToBlobContainerAsync(string data, BlobContainer blobContainer, BlobType blobType,
            string blobNameSuffix = null, string blobNamePrefix = null);
    }
}
