using System;
using System.Xml.Linq;
using DpConnect.Configuration;


namespace DpConnect.OpcUa
{
    public class OpcUaConnectionConfiguration : IDpConnectionConfiguration
    {
        [DpConfigProperty("ConnectionId")]
        public string ConnectionId { get; set; }

        [DpConfigProperty("Endpoint")]
        public string Endpoint { get; set; }

        [DpConfigProperty("Active")]
        public bool Active { get; set; } = true;

        public Type ConnectionType { get; private set; } = typeof(IOpcUaConnection);

        public void FromXml(XDocument config)
        {
            Endpoint = config.Root.Element("Endpoint").Value;
            ConnectionId = config.Root.Element("ConnectionId").Value;
        }
    }
}
