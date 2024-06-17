using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Blob
{
    public abstract class BaseBlobDataService
    {
        protected BlobContainerClient MandalayBlobContainerClient => GetBlobContainerClientAsync(BlobConnection, MandalayBlobContainer).Result;

        protected BlobContainerClient JdeBlobContainerClient => GetBlobContainerClientAsync(BlobConnection, JdeBlobContainer).Result;

        public string BlobConnection => Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        public string MandalayBlobContainer => Environment.GetEnvironmentVariable("MandalayTransactionsBlobContainer");

        public string JdeBlobContainer => Environment.GetEnvironmentVariable("JdeTransactionsBlobContainer");

        private async Task<BlobContainerClient> GetBlobContainerClientAsync(string blobConnection, string blobContainer)
        {
            var blobContainerClient = new BlobContainerClient(blobConnection, blobContainer);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer).ConfigureAwait(false);
            return blobContainerClient;
        }
    }
}
