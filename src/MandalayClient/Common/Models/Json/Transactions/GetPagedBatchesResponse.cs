﻿using Newtonsoft.Json;

namespace MandalayClient.Common.Models.Json.Transactions
{
    public class GetPagedBatchesResponse : BatchStatusResponse
    {
        [JsonProperty(PropertyName = "value")]
        public PagedBatch PagedBatch;
    }
}
