using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breas.Device.Communication
{
    public class LengthException: CommunicationException
    {
        public LengthException(string message)
            : base(message)
        {
        }

        public LengthException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
