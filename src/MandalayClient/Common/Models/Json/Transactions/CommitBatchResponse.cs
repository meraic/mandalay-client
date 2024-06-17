﻿using Newtonsoft.Json;

namespace MandalayClient.Common.Models.Json.Transactions
{
    public class CommitBatchResponse : BatchStatusResponse 
    {
        [JsonProperty(PropertyName = "value")]
        public int? Value;
    }
}
