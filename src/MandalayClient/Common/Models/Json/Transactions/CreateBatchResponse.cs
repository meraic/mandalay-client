﻿using Newtonsoft.Json;

namespace MandalayClient.Common.Models.Json.Transactions
{
    public class CreateBatchResponse : BatchStatusResponse
    {
        [JsonProperty(PropertyName = "value")]
        public Batch Batch;
    }
}
