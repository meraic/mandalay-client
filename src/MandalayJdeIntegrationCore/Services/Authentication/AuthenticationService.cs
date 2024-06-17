using MandalayClient.Common;
using MandalayClient.Common.Extensions;
using System;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private string authRequestTokenUrl;
        private string clientId;
        private string clientSecret;
        private string apiId;
        private string userId;
        private string password;
        private string billingExtractApiSubscriptionKey;
        private string csIntegrationApiSubscriptionKey;
        private string apiGatewayUrl;
        private string txnApiEndPoint;

        public async Task<AuthenticationClient> AuthenticateClient()
        {
            using (var authClient = new AuthenticationClient())
            {
                InitConfig();
                var useDefaultTestAuthTokenRequestUrl = authRequestTokenUrl.IsNullOrEmpty();

                if (useDefaultTestAuthTokenRequestUrl)
                {
                    await authClient.ClientCredentialsAsync(clientId, clientSecret);
                }
                else
                {
                    await authClient.ClientCredentialsAsync(clientId, clientSecret, authRequestTokenUrl);
                }

                authClient.BillingExtractSubscriptionKey = billingExtractApiSubscriptionKey;
                authClient.CSIntegrationSubscriptionKey = csIntegrationApiSubscriptionKey;
                authClient.ApiGatewayUrl = apiGatewayUrl;
                authClient.TransactionExportApiEndPoint = txnApiEndPoint;

                return authClient;
            }
        }

        public async Task<AuthenticationClientByResourceOwner> AuthenticationClientByResourceOwner()
        {
            using (var authClient = new AuthenticationClientByResourceOwner())
            {
                InitConfig();
                var useDefaultTestAuthTokenRequestUrl = authRequestTokenUrl.IsNullOrEmpty();

                if (useDefaultTestAuthTokenRequestUrl)
                {
                    await authClient.UsernamePasswordAsync(apiId, userId, password);
                }
                else
                {
                    await authClient.UsernamePasswordAsync(apiId, userId, password, authRequestTokenUrl);
                }

                authClient.BillingExtractSubscriptionKey = billingExtractApiSubscriptionKey;
                authClient.CSIntegrationSubscriptionKey = csIntegrationApiSubscriptionKey;
                authClient.ApiGatewayUrl = apiGatewayUrl;
                authClient.TransactionExportApiEndPoint = txnApiEndPoint;

                return authClient;
            }
        }

        private void InitConfig()
        {
            authRequestTokenUrl = Environment.GetEnvironmentVariable("MandalayAuthRequestTokenUrl");
            clientId = Environment.GetEnvironmentVariable("MandalayClientId"); 
            clientSecret = Environment.GetEnvironmentVariable("MandalayClientSecret");
            apiId = Environment.GetEnvironmentVariable("MandalayApiId");
            userId = Environment.GetEnvironmentVariable("MandalayUserId");
            password = Environment.GetEnvironmentVariable("MandalayPassword");
            billingExtractApiSubscriptionKey = Environment.GetEnvironmentVariable("MandalayBillingExtractApiSubscriptionKey");
            csIntegrationApiSubscriptionKey = Environment.GetEnvironmentVariable("MandalayCSIntegrationApiSubscriptionKey");
            apiGatewayUrl = Environment.GetEnvironmentVariable("MandalayApiGatewayUrl");
            txnApiEndPoint = Environment.GetEnvironmentVariable("MandalayTransactionsExportApiEndPoint");
        }
    }
}
