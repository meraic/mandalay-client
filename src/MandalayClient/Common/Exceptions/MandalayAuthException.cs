using MandalayClient.Common.Models.Json;
using System;
using System.Net;

namespace MandalayClient.Common.Exceptions
{
    public class MandalayAuthException : Exception
    {
        public string[] Fields { get; private set; }
        public HttpStatusCode HttpStatusCode { get; private set; }
        public ErrorCode Error { get; private set; }

        public MandalayAuthException(string error)
            : this(ParseError(error))
        {
        }

        public MandalayAuthException(string error, string[] fields)
            : this(error)
        {
            Fields = fields;
        }

        public MandalayAuthException(ErrorCode error, string[] fields)
            : this(error)
        {
            Fields = fields;
        }

        public MandalayAuthException(string error, HttpStatusCode httpStatusCode)
            : this(ParseError(error))
        {
            this.HttpStatusCode = httpStatusCode;
        }

        public MandalayAuthException(ErrorCode error)
            : base()
        {
            Error = error;
            Fields = new string[0];
            HttpStatusCode = new HttpStatusCode();
        }

        private static ErrorCode ParseError(string error)
        {
            ErrorCode value;
            return Enum.TryParse(error.Replace("_", ""), true, out value) ? value : ErrorCode.Unknown;
        }
    }
}
