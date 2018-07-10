using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breas.Device.Communication
{
    public class NackException : CommunicationException
    {
        public enum ResponseType
        {
            Ok,
            CRCError,
            LengthError,
            MessageParseError,
            AccessDenied,
            JobNotStarted,
            CommandNotSupported,
            NonValidData,
            TooManyExternalClients
        }

        private ResponseType _type;

        public ResponseType NackType
        {
            get { return _type; }
            set { _type = value; }
        }


        public NackException(string message)
            : base(message)
        {
        }

        public NackException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public NackException(string message, ResponseType type)
            : base(message)
        {
            _type = type;
        }
    }
}
