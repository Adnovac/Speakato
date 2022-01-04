using System;
using System.Net;

namespace Speakato.VoiceRecognizer
{
    public class VoiceRecognizerServiceException : Exception
    {
        public VoiceRecognizerServiceException()
        { }

        public VoiceRecognizerServiceException(string message)
            : base(message)
        { }

        public VoiceRecognizerServiceException(string message, Exception inner)
            : base(message, inner)
        { }

        public VoiceRecognizerServiceException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}
