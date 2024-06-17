using System;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Load
{
    public class DropFileToOnPremiseDestinationService : IDropFileToOnPremiseDestinationService
    {
        private string JdeCsvFileDestiationUrl => Environment.GetEnvironmentVariable("JdeCsvFileDestiationUrl");

        private string JdeCsvFileDestiationPath => Environment.GetEnvironmentVariable("JdeCsvFileDestiationPath");

        private string JdeCsvFileDestiationSignature => Environment.GetEnvironmentVariable("JdeCsvFileDestiationSignature");

        public async Task DropCsvToOnPremiseDestination(string csvData, string site, DateTime tenantLocalTime)
        {
            var url = $"{JdeCsvFileDestiationUrl}{JdeCsvFileDestiationPath}{JdeCsvFileDestiationSignature}";
            //TODO
            await Task.CompletedTask;
        }
    }
}
