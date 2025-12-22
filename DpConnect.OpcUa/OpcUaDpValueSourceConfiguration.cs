using DpConnect.Connection;
using System.Xml.Linq;

namespace DpConnect.OpcUa
{
    public class OpcUaDpValueSourceConfiguration : IDpSourceConfiguration
    {
        public string NodeId { get; set; }

        public void FromXml(XDocument config)
        {
            NodeId = config.Root.Element("NodeId").Value;
        }
    }
}
