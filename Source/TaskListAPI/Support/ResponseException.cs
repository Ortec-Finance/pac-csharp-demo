using System;
using System.Net;
using System.Runtime.Serialization;

namespace TaskListAPI
{
    [Serializable]
    public class ResponseException : Exception
    {
        public ResponseException(HttpStatusCode statusCode, string message) : this((int)statusCode, message)
        {
        }

        public ResponseException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        protected ResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public int StatusCode { get; }
    }
}