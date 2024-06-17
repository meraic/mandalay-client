using MandalayJdeIntegrationCore.Models;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using Azure.Storage.Blobs.Models;

namespace MandalayJdeIntegrationCore.Services.Blob
{
    public class BlobDataService : BaseBlobDataService, IBlobDataService
    {
        /// <summary>
        /// Export data to Azure Blob Container
        /// </summary>
        /// <param name="data">Json/Csv data to be uploaded to Blob Container</param>
        /// <param name="blob">Blob file name</param>
        /// <param name="blobContainer">Blob Container (Mandalay Container or Jde Container)</param>
        /// <returns></returns>
        public async Task ExportToBlobContainerAsync(string data, string blobName, BlobContainer blobContainer)
        {
            var blobContainerClient = blobContainer == BlobContainer.Mandalay
                ? MandalayBlobContainerClient
                : JdeBlobContainerClient;

            // Get Blob client for the specified Blob
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            // Delete blob if already exist
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

            // Streaming
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));

            // Upload blob
            await blobContainerClient.UploadBlobAsync(blobName, stream);
        }
    }
}
