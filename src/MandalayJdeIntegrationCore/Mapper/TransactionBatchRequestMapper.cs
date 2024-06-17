using MandalayClient.Common.Models.Json.Transactions;
using MandalayJdeIntegrationCore.Extensions;
using MandalayJdeIntegrationCore.Models;
using MandalayJdeIntegrationCore.Services.Mapping;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Mapper
{
    public class TransactionBatchRequestMapper : ITransactionBatchRequestMapper
    {
        private readonly IMappingDataService mappingDataService;
        private readonly string transactionBatchStatusFilter;

        public TransactionBatchRequestMapper(IMappingDataService mappingDataService)
        {
            this.mappingDataService = mappingDataService;
            transactionBatchStatusFilter = Environment.GetEnvironmentVariable("MandalayCreateBatchTransactionStatusFilter");
        }

        public async Task<NausTransactionBatchRequest> MapFrom(TransactionBatchRequest request)
        {
            var reolvedParams = ResolveRequest(request.Site, request.Tenant);
            var siteId = reolvedParams.Item1;
            var tenantId = reolvedParams.Item2;
            var tenantLocalTime = reolvedParams.Item3;

            var filterEndDate = ResolveRequest(request.EndDate);

            var nausTxnBatchRequest = new NausTransactionBatchRequest
            {
                SiteId = siteId,
                TenantId = tenantId,
                PeriodEndDate = filterEndDate,
                TransactionStatus = transactionBatchStatusFilter?.ToEnum(TransactionStatus.All) ?? TransactionStatus.All,
                TenancyLocalTime = tenantLocalTime
            };

            return await Task.FromResult(nausTxnBatchRequest);
        }

        private Tuple<string, string, DateTime> ResolveRequest(string site, string tenant)
        {
            string siteId;
            string tenantId;

            if (site.IsNullOrEmpty() || site.FilterAll())
            {
                var tenantMappings = mappingDataService.TenantMappings;

                if (tenant.IsNullOrEmpty()) throw new Exception("Tenant is required in the request when Site is not provided or Site is ALL");
                 
                if (tenantMappings.Count == 0) throw new Exception("There are no tenants configured in Tenant mapping table");

                var tenantMapping = tenantMappings
                    .Where(tm => tm.Key.Equals(tenant.Trim(), StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                siteId = null;
                tenantId = tenantMapping.Value?.MandalayTenantId?.Trim() ??
                     throw new Exception($"Tenant: {tenant} specified in the request is not configured in Tenant mapping table.");
                    
            }
            else
            {
                var siteMappings = mappingDataService.SiteMappings;

                if (siteMappings.Count == 0) throw new Exception("There are no sites configured in Site mapping table");

                var siteMapping = siteMappings
                    .Where(sm => sm.Key.Equals(site.Trim(), StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                siteId = siteMapping.Value?.MandalaySiteId?.Trim() ?? 
                    throw new Exception($"Site: {site} specified in the request is not configured in Site mapping table.");

                tenantId = siteMapping.Value?.MandalayTenantId?.Trim() ??
                    throw new Exception($"Site: {site} specified in the request is not configured in Site mapping table.");
            }

            var tenantLocalTimeZone = mappingDataService.TenantMappings.FirstOrDefault(m => m.Key == tenant);
            var tenantLocalTime = ResolveTenantTimeByTimeZone(tenantLocalTimeZone.Value?.TimeZone);

            var tuple = Tuple.Create(siteId, tenantId, tenantLocalTime);

            return tuple;
        }

        private string ResolveRequest(string endDate)
        {
            //TODO
            return null;
        }

        private DateTime ResolveTenantTimeByTimeZone(string timeZoneValue)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneValue);
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            return localTime;
        }
    }
}
