

using System;
using System.Xml.Linq;

namespace DpConnect
{
    public interface IDpConnectionConfiguration
    {

        Type ConnectionType { get; }

        string ConnectionId { get; }

        bool Active { get; }        

        void FromXml(XDocument config);
    }
}
