using MandalayClient.Common.Models.Json.Transactions;
using Newtonsoft.Json;

namespace MandalayJdeIntegrationCore.Extensions
{
    public static class BatchRequestExtensions
    {
        public static string ToJson(this CreateBatchRequest request)
        {
            return JsonConvert.SerializeObject(request);
        }

        public static string ToJson(this CreateBatchResponse response)
        {
            return JsonConvert.SerializeObject(response);
        }
    }
}
