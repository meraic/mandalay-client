using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MandalayJdeIntegrationCore.Models;
using MandalayJdeIntegrationCore.Exceptions;
using System;
using MandalayClient.Common.Models.Json.Transactions;
using MandalayJdeIntegrationCore.Services.Discard;
using MandalayJdeIntegrationCore.Services.Extraction;
using MandalayJdeIntegrationCore.Services.Load;
using MandalayJdeIntegrationCore.Services.Transformation;
using System.Linq;
using MandalayJdeIntegrationCore.Extensions;

namespace MandalayJdeIntegration
{
    public partial class Transactions
    {
        private readonly ITransactionBatchService txnBatchService;
        private readonly IJdeTransformationService jdeTransformationService;
        private readonly ILoadToJdeService loadToJdeService;
        private readonly IDiscardBatchService discardBatchService;

        public Transactions(
            ITransactionBatchService txnBatchService,
            IJdeTransformationService jdeTransformationService,
            ILoadToJdeService loadToJdeService,
            IDiscardBatchService discardBatchService)
        {
            this.txnBatchService = txnBatchService;
            this.jdeTransformationService = jdeTransformationService;
            this.loadToJdeService = loadToJdeService;
            this.discardBatchService = discardBatchService;
        }

        [FunctionName("Transactions")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "transactions")] HttpRequest req,
            ILogger logger)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            logger.LogInformation($"Request Body: {requestBody}");

            // Get the transaction batch request from request body
            var txnBatchRequest = JsonConvert.DeserializeObject<TransactionBatchRequest>(requestBody);
            var txnBatchResponse = default(TransactionBatchResponse);

            try
            {
                // Extract transactions from Mandalay
                logger.LogInformation($"Extracting transactions from Mandalay...Started");
                txnBatchResponse = await txnBatchService.GetTransactionBatchAsync(txnBatchRequest);
                logger.LogInformation($"Extracting transactions from Mandalay...Completed");

                // Transorm transactions from Mandalay to Jde
                logger.LogInformation($"Transforming transactions from Mandalay to Jde...Started");
                txnBatchResponse = await jdeTransformationService.TransformToJdeAsync(txnBatchResponse);
                logger.LogInformation($"Transforming transactions from Mandalay to Jde...Completed");

                // Send transformed transactions to Jde
                logger.LogInformation($"Exporting translated transactions to Jde...Started");
                await loadToJdeService.SendTransactionsToJdeAsync(txnBatchResponse);
                logger.LogInformation($"Exporting translated transactions to Jde...Completed");

                // Currently we are discarding till we go live as once transactions are commited, we can't get them in batch.
                // Service to Commit transaction is ready but needs to be tested once we are ok with environment/data testing
                logger.LogInformation($"Commiting transaction batch...Started");
                await discardBatchService.DiscardBatchAsync(txnBatchResponse.BatchId, txnBatchResponse.TenantId);
                logger.LogInformation($"Commiting transaction batch...Completed");

                // Send Notifications to Business
                // TODO

                // Return response
                return new OkObjectResult(txnBatchResponse.ToResponse(txnBatchRequest));
            }
            catch(BadRequestException ex)
            {
                logger.LogError(ex.Message);
                return new BadRequestObjectResult(new { Message = "Bad Request" });
            }
            catch (TransactionBatchException ex)
            {
                // Log exception message 
                logger.LogError(ex.Message);
                txnBatchResponse = ex.TransactionBatchResponse;
                if (txnBatchResponse.HasErrors && txnBatchResponse.BatchStatus == BatchStatus.Created && !txnBatchResponse.BatchId.IsNullOrEmpty())
                {
                    // Discard batch as batch is created but not completed
                    logger.LogInformation($"Discarding transaction batch...Started");
                    await discardBatchService.DiscardBatchAsync(txnBatchResponse.BatchId, txnBatchResponse.TenantId);
                    logger.LogInformation($"Discarding transaction batch...Completed");
                }

                return new ObjectResult(txnBatchResponse.ToResponse(txnBatchRequest, "Error"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
