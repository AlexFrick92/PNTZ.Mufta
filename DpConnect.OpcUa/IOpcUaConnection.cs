

using DpConnect.Connection;

namespace DpConnect.OpcUa
{
    public interface IOpcUaConnection :        
        IDpConfigurableConnection<OpcUaConnectionConfiguration>,
        IDpBindableConnection<OpcUaDpValueSourceConfiguration>
    {

    }
}
