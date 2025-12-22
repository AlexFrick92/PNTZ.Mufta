using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpConnect.Exceptions
{
    public class TransportLevelDpException : Exception
    {
        public TransportLevelDpException(string message) : base(message) { }

        public TransportLevelDpException(string message, Exception innerException) : base(message, innerException) { }
    }
}
