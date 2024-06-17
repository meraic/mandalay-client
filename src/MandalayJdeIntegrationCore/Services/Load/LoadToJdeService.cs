using MandalayJdeIntegrationCore.Exceptions;
using MandalayJdeIntegrationCore.Extensions;
using MandalayJdeIntegrationCore.Models;
using MandalayJdeIntegrationCore.Services.Blob;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Load
{
    public class LoadToJdeService : ILoadToJdeService
    {
        private readonly IExportToBlobContainerService exportToBlobContainerService;
        private readonly IDropFileToOnPremiseDestinationService dropFileToOnPremiseDestinationService;
        private readonly ILogger<ILoadToJdeService> logger;

        public LoadToJdeService(
            IExportToBlobContainerService exportToBlobContainerService, 
            IDropFileToOnPremiseDestinationService dropFileToOnPremiseDestinationService, 
            ILogger<ILoadToJdeService> logger)
        {
            this.exportToBlobContainerService = exportToBlobContainerService;
            this.dropFileToOnPremiseDestinationService = dropFileToOnPremiseDestinationService;
            this.logger = logger;
        }

        public async Task<TransactionBatchResponse> SendTransactionsToJdeAsync(TransactionBatchResponse txnBatchResponse)
        {
            try
            {
                var jdeTransactions = txnBatchResponse.JdeTransactions;
                var blobNameSuffix = txnBatchResponse.BatchId;

                logger.LogInformation("Exporting JDE Transactions CSV file to Azure Blob Storage Container...Started");

                if (!txnBatchResponse.JdeTransactionsCsv.IsNullOrEmpty())
                {
                    await exportToBlobContainerService.ExportTransactionsToBlobContainerAsync(
                        txnBatchResponse.JdeTransactionsCsv,
                        BlobContainer.Jde, 
                        BlobType.Csv, 
                        null,
                        txnBatchResponse.Site);
                }

                logger.LogInformation("Exporting JDE Transactions CSV file to Azure Blob Storage Container...Completed");

                /*logger.LogInformation("Droping JDE Transactions CSV file to JDE Directory...Started");
                await dropFileToOnPremiseDestinationService.DropCsvToOnPremiseDestination(
                    txnBatchResponse.JdeTransactionsCsv,
                    txnBatchResponse.Site,
                    txnBatchResponse.TenantLocalDateTime);
                logger.LogInformation("Droping JDE Transactions CSV file to JDE Directory...Completed");*/

                return txnBatchResponse;
            }
            catch(Exception ex)
            {
                txnBatchResponse.HasErrors = true;
                txnBatchResponse.Errors.Add(ex.Message);
                throw new TransactionBatchException(ex.Message, txnBatchResponse);
            }
        }
    }
}
