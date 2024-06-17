using Newtonsoft.Json;

namespace MandalayClient.Common.Models.Json
{
    public class Error
    {
        [JsonProperty(PropertyName = "field")]
        public string Field;

        [JsonProperty(PropertyName = "message")]
        public string Message;
    }
}
