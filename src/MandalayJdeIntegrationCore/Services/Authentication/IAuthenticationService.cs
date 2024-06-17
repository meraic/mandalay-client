using MandalayClient.Common;
using System.Threading.Tasks;

namespace MandalayJdeIntegrationCore.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticationClient> AuthenticateClient();
        Task<AuthenticationClientByResourceOwner> AuthenticationClientByResourceOwner();
    }
}
