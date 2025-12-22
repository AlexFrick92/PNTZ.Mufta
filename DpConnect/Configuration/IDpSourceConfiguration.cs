
using System.Xml.Linq;

namespace DpConnect
{
    public interface IDpSourceConfiguration
    {
        void FromXml(XDocument config);
    }
}
