﻿using MandalayClient.Common.Models.Json.Transactions;
using System;
using System.Threading.Tasks;

namespace MandalayClient
{
    public interface INausTransactionExportApiClient : IDisposable
    {
        Task<CreateBatchResponse> CreateBatchAsync(CreateBatchRequest request);
        Task<GetBatchResponse> GetBatchAsync(GetBatchRequest request);
        Task<GetPagedBatchesResponse> GetPagedBatchesAsync(GetPagedBatchesRequest request);
        Task<DiscardBatchResponse> DiscardBatchAsync(DiscardBatchRequest request);
        Task<CommitBatchResponse> CommitBatchAsync(CommitBatchRequest request);
    }
}
