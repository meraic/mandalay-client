using Newtonsoft.Json;

namespace MandalayClient.Common.Models.Json
{
    public class AuthResourceErrorResponse
    {
        [JsonProperty(PropertyName = "error")]
        public string Error;
    }
}
