using System;
using System.Collections;

namespace MandalayClient.Common.Exceptions
{
    public interface IMandalayException
    {
        Exception GetBaseException();
        string ToString();
        IDictionary Data { get; }
        Exception InnerException { get; }
        string Message { get; }
        string StackTrace { get; }
    }
}
