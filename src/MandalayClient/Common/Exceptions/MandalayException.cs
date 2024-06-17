using MandalayClient.Common.Models.Json;
using System;
using System.Linq;
using System.Net;

namespace MandalayClient.Common.Exceptions
{
    public class MandalayException : Exception, IMandalayException
    {
        public string[] Errors { get; private set; }

        public HttpStatusCode HttpStatusCode { get; private set; }

        public MandalayException(Error[] errors, string message, HttpStatusCode httpStatusCode)
            : base(ParseErrors(message, errors))
        {
            Errors = errors.Select(e => $"{e.Field} - {e.Message}").ToArray();
            HttpStatusCode = httpStatusCode;
        }

        public MandalayException(string message, HttpStatusCode httpStatusCode)
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        private static string ParseErrors(string message, Error[] errors)
        {
            return errors.Any()
                ? $"{message} - {string.Join(Environment.NewLine, errors.Select(e => $"{e.Field} - {e.Message}"))}"
                : message;
        }
    }
}
