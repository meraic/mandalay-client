using System;
using System.Threading.Tasks;

namespace MandalayClient.Common
{
    public interface IAuthenticationClient : IDisposable
    {
        string TokenType { get; set; }
        string RefreshToken { get; set; }
        string AccessToken { get; set; }
        long TokenExpiry { get; set; }

        Task ClientCredentialsAsync(string clientId, string clientSecret);
        Task ClientCredentialsAsync(string clientId, string clientSecret, string tokenRequestEndpointUrl);
    }
}
