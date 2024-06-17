using MandalayClient;
using MandalayJdeIntegration;
using MandalayJdeIntegrationCore.Mapper;
using MandalayJdeIntegrationCore.Services.Authentication;
using MandalayJdeIntegrationCore.Services.Blob;
using MandalayJdeIntegrationCore.Services.Discard;
using MandalayJdeIntegrationCore.Services.Extraction;
using MandalayJdeIntegrationCore.Services.Load;
using MandalayJdeIntegrationCore.Services.Notificaiton;
using MandalayJdeIntegrationCore.Services.Transformation;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MandalayJdeIntegrationCore.Services.Mapping;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MandalayJdeIntegration
{
    public class Startup : FunctionsStartup
    {
        /// <summary>
        /// Register dependencies in container as a part of starting up the host runtime.
        /// </summary>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

            builder.Services.AddSingleton<INausTransactionExportApiClient, NausTransactionExportApiClient>(serviceProvider => 
                GetNausTransactionExportApiClient());

            builder.Services.AddSingleton<IMappingDataService, MappingDataService>();

            builder.Services.AddSingleton<INotifyService, NotifyService>();

            builder.Services.AddSingleton<IBlobDataService, BlobDataService>();

            builder.Services.AddSingleton<IExportToBlobContainerService, ExportToBlobContainerService>();

            builder.Services.AddSingleton<ITransactionBatchService, TransactionBatchService>();

            builder.Services.AddSingleton<IJdeTransformationService, JdeTransformationService>();

            builder.Services.AddSingleton<ILoadToJdeService, LoadToJdeService>();

            builder.Services.AddSingleton<ITransactionBatchRequestMapper, TransactionBatchRequestMapper>();

            builder.Services.AddSingleton<IJdeTransformationService, JdeTransformationService>();

            builder.Services.AddSingleton<IDiscardBatchService, DiscardBatchService>();
        }

        private NausTransactionExportApiClient GetNausTransactionExportApiClient()
        {
            var authService = new AuthenticationService();

            // Register Mandalay Authentication Client to register into dependency injection container. 
            // Here we authorize using grant_type = client_credentials
            var authClient = authService.AuthenticateClient().Result;
            return new NausTransactionExportApiClient(
                authClient.ApiGatewayUrl,
                authClient.TransactionExportApiEndPoint,
                authClient.AccessToken,
                authClient.BillingExtractSubscriptionKey);
        }
    }
}
