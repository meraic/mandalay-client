using MandalayJdeIntegrationCore.Exceptions;
using MandalayJdeIntegrationCore.Helper;
using MandalayJdeIntegrationCore.Models;
using MandalayJdeIntegrationCore.Services.Mapping;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Transformation
{
    public class JdeTransformationService : IJdeTransformationService
    {
        private readonly ILogger<IJdeTransformationService> logger;
        private readonly IMappingDataService mappingDataService;

        public JdeTransformationService(IMappingDataService mappingDataService, ILogger<IJdeTransformationService> logger)
        {
            this.mappingDataService = mappingDataService;
            this.logger = logger;
        }

        public async Task<TransactionBatchResponse> TransformToJdeAsync(TransactionBatchResponse txnBatchResponse)
        {
            try
            {
                var mandalayTxns = txnBatchResponse.Transactions;

                // Please note that this piece of code is complete shit and needs to replaced at some point and it is done fro POC purpose
                var allCsvData = mandalayTxns.MapToCsv(txnBatchResponse.Site, txnBatchResponse.Tenant, txnBatchResponse.TenantLocalDateTime,
                    mappingDataService.SiteMappings,
                    mappingDataService.TenantMappings,
                    mappingDataService.UomMappings);

                var csvData = allCsvData.CsvLines;

                txnBatchResponse.JdeTransactionsCsv = string.Join(Environment.NewLine, allCsvData.CsvLines);

                return await Task.FromResult(txnBatchResponse);
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
