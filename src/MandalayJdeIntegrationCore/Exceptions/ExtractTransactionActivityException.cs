using MandalayClient.Common.Models.Json.Transactions;
using MandalayJdeIntegrationCore.Models;
using System;

namespace MandalayJdeIntegrationCore.Exceptions
{
    public class ExtractTransactionActivityException : Exception
    {
        public NausTransactionBatchRequest NausTransactionBatchRequest { get; private set; }

        public NausTransactionBatchResponse NausTransactionBatchResponse { get; private set; }

        public TransactionBatchResponse TransactionBatchResponse { get; private set; }

        public ExtractTransactionActivityException(string message,
            NausTransactionBatchRequest nausTxnBatchRequest,
            NausTransactionBatchResponse nausTxnBatchResponse, 
            TransactionBatchResponse txnBatchResponse) : base(message)
        {
            this.NausTransactionBatchRequest = nausTxnBatchRequest;
            this.NausTransactionBatchResponse = nausTxnBatchResponse;
            this.TransactionBatchResponse = txnBatchResponse;
        }
    }
}
