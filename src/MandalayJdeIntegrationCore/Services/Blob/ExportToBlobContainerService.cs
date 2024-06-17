using MandalayClient.Common.Models.Json.Transactions;
using MandalayJdeIntegrationCore.Extensions;
using MandalayJdeIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Blob
{
    public class ExportToBlobContainerService : IExportToBlobContainerService
    {
        private readonly IBlobDataService blobDataService;

        public ExportToBlobContainerService(IBlobDataService blobDataService)
        {
            this.blobDataService = blobDataService;
        }

        public async Task ExportTransactionsToBlobContainerAsync(string data, BlobContainer blobContainer, BlobType blobType,
            string blobNameSuffix = null, string blobNamePrefix = null)
        {
            var blobName = GetBlobName(blobNameSuffix, blobType, blobNamePrefix);

            await blobDataService.ExportToBlobContainerAsync(data, blobName, blobContainer).ConfigureAwait(false);
        }

        public async Task ExportTransactionsToBlobContainerAsync<T>(IEnumerable<T> data, BlobContainer blobContainer, BlobType blobType,
            string blobNameSuffix = null, string blobNamePrefix = null)
        {
            var dataUploadToBlobContainer = blobType == BlobType.Csv ? data.ToCsv() : data.ToJson();

            var blobName = GetBlobName(data.FirstOrDefault().GetType(), blobNameSuffix, blobType, blobNamePrefix); 

            await blobDataService.ExportToBlobContainerAsync(dataUploadToBlobContainer, 
                blobName, blobContainer).ConfigureAwait(false);
        }

        private string GetBlobName<T>(T type, string blobNameSuffix, BlobType blobType, string blobNamePrefix = null)
        {
            var utcNow = TimeZoneInfo .ConvertTimeFromUtc(DateTime.UtcNow, 
                TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"));
            
            var blobName = typeof(T) == typeof(Transaction)
                ? Environment.GetEnvironmentVariable("MandalayTransactionsBlobNameFormat")
                : Environment.GetEnvironmentVariable("JdeTransactionsBlobNameFormat");


            blobName = blobType == BlobType.Csv ? $"{blobName}.csv" : $"{blobName}.json";

            blobName = blobName.Replace("{datetimeinfo}", utcNow.ToString("yyyyMMdd-HHmmss"));

            blobName = blobNameSuffix.IsNullOrEmpty() ? blobName : blobName.Replace("-{uuid}", blobNameSuffix);

            blobName = blobNamePrefix.IsNullOrEmpty() ? blobName : $"{blobNamePrefix}-{blobName}";

            return blobName;
        }

        private string GetBlobName(string blobNameSuffix, BlobType blobType, string blobNamePrefix = null)
        {
            var utcNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"));

            var blobName = Environment.GetEnvironmentVariable("JdeTransactionsBlobNameFormat");

            blobName = blobType == BlobType.Csv ? $"{blobName}.csv" : $"{blobName}.json";

            blobName = blobName.Replace("{datetimeinfo}", utcNow.ToString("yyyyMMdd-HHmmss"));

            blobName = blobNameSuffix.IsNullOrEmpty()
                ? blobName.Replace("-{uuid}", string.Empty)
                : blobName.Replace("-{uuid}", blobNameSuffix);

            blobName = blobNamePrefix.IsNullOrEmpty() ? blobName : $"{blobNamePrefix}-{blobName}";

            return blobName;
        }
    }
}
