using MandalayJdeIntegrationCore.Models;
using System;

namespace MandalayJdeIntegrationCore.Exceptions
{
    public class TransactionBatchException : Exception
    {
        public TransactionBatchRequest TransactionBatchRequest { get; private set; }

        public TransactionBatchResponse TransactionBatchResponse { get; private set; }

        public TransactionBatchException(string message, 
            TransactionBatchRequest transactionBatchRequest, TransactionBatchResponse transactionBatchResponse) 
            : base(message)
        {
            this.TransactionBatchRequest = transactionBatchRequest;
            this.TransactionBatchResponse = transactionBatchResponse;
        }

        public TransactionBatchException(string message, TransactionBatchResponse transactionBatchResponse)
            : base(message)
        {
            this.TransactionBatchResponse = transactionBatchResponse;
        }
    }
}
