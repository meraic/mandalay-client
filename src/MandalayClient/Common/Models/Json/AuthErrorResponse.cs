using Newtonsoft.Json;

namespace MandalayClient.Common.Models.Json
{
    public class AuthErrorResponse
    {
        [JsonProperty(PropertyName = "error")]
        public string Error;
    }
}
