using MandalayClient;
using MandalayClient.Common.Models.Json.Transactions;
using MandalayJdeIntegrationCore.Exceptions;
using MandalayJdeIntegrationCore.Extensions;
using MandalayJdeIntegrationCore.Mapper;
using MandalayJdeIntegrationCore.Models;
using MandalayJdeIntegrationCore.Services.Blob;
using MandalayJdeIntegrationCore.Services.Discard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Extraction
{
    public class TransactionBatchService : ITransactionBatchService
    {
        private readonly ILogger<ITransactionBatchService> logger;
        private readonly INausTransactionExportApiClient nausTxnExportApiClient;
        private readonly ITransactionBatchRequestMapper txnBatchRequestMapper;
        private readonly IExportToBlobContainerService exportToBlobContainerService;
        private readonly IDiscardBatchService discardBatchService;
        private readonly int pageSize;

        public TransactionBatchService(
            INausTransactionExportApiClient nausTxnExportApiClient, 
            ITransactionBatchRequestMapper txnBatchRequestMapper,
            IExportToBlobContainerService exportToBlobContainerService,
            IDiscardBatchService discardBatchService,
            ILogger<ITransactionBatchService> logger)
        {
            this.nausTxnExportApiClient = nausTxnExportApiClient;
            this.txnBatchRequestMapper  = txnBatchRequestMapper;
            this.pageSize = Environment.GetEnvironmentVariable("MandalayTransactionBatchPageSize")?.ToInteger() ?? 1000;
            this.exportToBlobContainerService = exportToBlobContainerService;
            this.discardBatchService = discardBatchService;
            this.logger = logger;
        }

        public async Task<TransactionBatchResponse> GetTransactionBatchAsync(TransactionBatchRequest request)
        {
            var nausTxnBatchRequest = new NausTransactionBatchRequest();
            var nausTxnBatchResponse = new CreateBatchResponse();
            var txnBatchResponse = new TransactionBatchResponse();

            try
            {
                //Step 1: Map the request sent from function to Mandalay batch request to start the first step i e create batch
                logger.LogInformation("Resolving request...Started");
                nausTxnBatchRequest = await txnBatchRequestMapper.MapFrom(request).ConfigureAwait(false);
                logger.LogInformation("Resolving request...Completed");

                logger.LogInformation($"Resolved Request: {nausTxnBatchRequest.ToJson()}");
                txnBatchResponse.TenantId = nausTxnBatchRequest.TenantId;
                txnBatchResponse.SiteId = nausTxnBatchRequest.SiteId;
                txnBatchResponse.Site = request.Site;
                txnBatchResponse.Tenant = request.Tenant;
                txnBatchResponse.TenantLocalDateTime = nausTxnBatchRequest.TenancyLocalTime;

                //Step 2: Send the Create Transactions Batch request to Mandalay
                logger.LogInformation("Sending Create Batch request to Mandalay...Started");
                nausTxnBatchResponse = await nausTxnExportApiClient.CreateBatchAsync(nausTxnBatchRequest).ConfigureAwait(false);
                logger.LogInformation("Sending Create Batch request to Mandalay...Completed");

                logger.LogInformation($"Create Batch Response from Mandalay: {nausTxnBatchResponse.ToJson()}");
                txnBatchResponse.BatchId = nausTxnBatchResponse?.Batch?.Id;
                txnBatchResponse.BatchStatus = nausTxnBatchResponse?.Batch?.Status ?? BatchStatus.Unknown;

                //Step 3: Get Batch if transactions in batch are generated, else no transactions
                logger.LogInformation("Get Batch request from Mandalay...Started");
                var getBatchResponse =  await GetBatchAsync(nausTxnBatchResponse);
                logger.LogInformation("Get Batch request from Mandalay...Completed");

                logger.LogInformation($"Transations Count (Retrieved from Mandalay): {getBatchResponse.Transactions?.Count()}");
                txnBatchResponse.Transactions = getBatchResponse.Transactions;

                return txnBatchResponse;
            }
            catch (Exception ex)
            {
                txnBatchResponse.HasErrors = true;
                txnBatchResponse.Errors.Add(ex.Message);
                throw new TransactionBatchException(ex.Message, request, txnBatchResponse);
            }
        }

        public async Task<bool> IsBatchCreatedWithTransactionsAsync(string tenantId, string batchId)
        {
            var areTransactionsGenerated = false;
            int reTryCount = 0;

            var getBatchRequest = new GetBatchRequest
            {
                TenantId = tenantId,
                BatchId = batchId,
                PageNumber = 0,
                PageSize = pageSize
            };

            var getBatchResponse = new GetBatchResponse();

            do
            {
                await Task.Delay(TimeSpan.FromSeconds(10));

                // Get Batch to retrieve the status of transactions in batch
                getBatchResponse = await nausTxnExportApiClient.GetBatchAsync(getBatchRequest).ConfigureAwait(false);

                // Check if the status of transactions in batch is generated 
                areTransactionsGenerated = getBatchResponse?.Batch?.FilteredResults?.Status == BatchStatus.Generated.ToString();

                // If transactions are generated in batch or retried to check status 5 times, stop retrying and exit
                if (areTransactionsGenerated || reTryCount >= 5)
                {
                    return areTransactionsGenerated;
                }

                // else wait for 5 seconds
                await Task.Delay(TimeSpan.FromSeconds(5));

                ++reTryCount;

            } while (!areTransactionsGenerated);

            return areTransactionsGenerated;
        }

        public async Task<bool> IsBatchCreatedByPagedBatchesAsync(string tenantId, string batchId)
        {
            var checkBatchCreatedRequest = new GetPagedBatchesRequest
            {
                TenantId = tenantId,
                BatchStatus = BatchStatus.Created
            };

            var pagedBatchesResponse = await nausTxnExportApiClient.GetPagedBatchesAsync(checkBatchCreatedRequest).ConfigureAwait(false);

            var isBatchCreated = pagedBatchesResponse?.PagedBatch?.Batches?.Any(batch =>
                batch.Id == batchId &&
                batch.TenantId == tenantId &&
                batch.FilteredResults.Status == BatchStatus.Generated.ToString()) ?? false;

            while (!isBatchCreated)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                pagedBatchesResponse = await nausTxnExportApiClient.GetPagedBatchesAsync(checkBatchCreatedRequest).ConfigureAwait(false);
                isBatchCreated = pagedBatchesResponse?.PagedBatch?.Batches?.Any(batch =>
                    batch.Id == batchId &&
                    batch.TenantId == tenantId &&
                    batch.FilteredResults.Status == BatchStatus.Generated.ToString()) ?? false;
            }

            return isBatchCreated;
        }

        public async Task<IEnumerable<Transaction>> GetCreatedBatchTransactionsAsync(string tenantId, string batchId)
        {
            int pageNumber = 0;
            int totalPages;
            var transactions = new List<Transaction>();

            do
            {
                var batchResponse = await nausTxnExportApiClient.GetBatchAsync(new GetBatchRequest
                {
                    TenantId = tenantId,
                    BatchId = batchId,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

                totalPages = batchResponse.TotalPageCount ?? 0;

                transactions.AddRange(batchResponse.Transactions);

            } while (++pageNumber < totalPages);

            return transactions;
        }

        private async Task<TransactionBatchResponse> GetBatchAsync(CreateBatchResponse batchResponse)
        {
            var tenantId = batchResponse?.Batch?.TenantId;
            var batchId = batchResponse?.Batch?.Id;
            var siteId = batchResponse?.Batch?.SiteId;

            var isBatchCreatedWithTxns = await IsBatchCreatedWithTransactionsAsync(tenantId, batchId).ConfigureAwait(false);

            // Create transaction batch response
            var txnBatchResponse = new TransactionBatchResponse
            {
                BatchId = batchId,
                TenantId = tenantId,
                SiteId = siteId,
            };

            if (isBatchCreatedWithTxns)
            {
                txnBatchResponse.BatchStatus = BatchStatus.Generated;
                var txns = await GetCreatedBatchTransactionsAsync(tenantId, batchId).ConfigureAwait(false);
                txnBatchResponse.Transactions = txns;

                // Export the transaction data as csv as well json to Mandalay blob container
                if (txns.Count() > 0)
                {
                    await exportToBlobContainerService.ExportTransactionsToBlobContainerAsync(txns,
                        BlobContainer.Mandalay, BlobType.Csv, batchId);
                    await exportToBlobContainerService.ExportTransactionsToBlobContainerAsync(txns,
                        BlobContainer.Mandalay, BlobType.Json, batchId);
                }

                return txnBatchResponse;
            }

            // If Batch is not created with generated transaction, set batch transaction generated status to Unknown
            txnBatchResponse.BatchStatus = BatchStatus.Unknown;

            return txnBatchResponse;
        }

        public async Task DiscardBatchTransactionsAsync(string batchId, string tenantId)
        {
            if (string.IsNullOrEmpty(batchId)) throw new ArgumentNullException("batchId");
            if (string.IsNullOrEmpty(tenantId)) throw new ArgumentNullException("tenantId");

            await discardBatchService.DiscardBatchAsync(batchId, tenantId).ConfigureAwait(false);
        }
    }
}
