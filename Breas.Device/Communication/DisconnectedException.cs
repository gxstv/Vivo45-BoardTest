using System;

namespace Breas.Device.Communication
{
    public class DisconnectedException : CommunicationException
    {
        public DisconnectedException(string message)
            : base(message)
        {
        }

        public DisconnectedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}