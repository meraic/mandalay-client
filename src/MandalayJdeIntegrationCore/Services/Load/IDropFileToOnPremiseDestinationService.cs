using System;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Load
{
    public interface IDropFileToOnPremiseDestinationService
    {
        Task DropCsvToOnPremiseDestination(string csvData, string site, DateTime tenantLocalTime);
    }
}
