using System;

namespace Breas.Device.Vivo45.Messenger
{
    internal class NackException : Exception
    {
        private NackException.ResponseType responseType;
        private string v;

        public NackException()
        {
        }

        public NackException(string message) : base(message)
        {
        }

        public NackException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NackException(string v, NackException.ResponseType responseType)
        {
            this.v = v;
            this.responseType = responseType;
        }

        internal enum ResponseType
        {
        }
    }
}