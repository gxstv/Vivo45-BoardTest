using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breas.Device.Communication
{
    public class CrcException : CommunicationException
    {
        public CrcException(string message)
            : base(message)
        {
        }

        public CrcException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
