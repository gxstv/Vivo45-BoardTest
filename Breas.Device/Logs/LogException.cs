using System;

namespace Breas.Device.Logs
{
    public class LogException : Exception
    {
        public  LogStatus Status
        {
            get;
            private set;
        }

        public LogException(string message, LogStatus status)
            : base(message)
        {
            this.Status = status;
        }

        public LogException(string message, LogStatus status, Exception innerException) 
            : base(message, innerException)
        {
            this.Status = status;
        }
    }
}

