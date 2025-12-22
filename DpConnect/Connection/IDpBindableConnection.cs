using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpConnect.Connection
{
    public interface IDpBindableConnection<TSourceConfiguration> : IDpConnection
        where TSourceConfiguration : IDpSourceConfiguration
    {
        void ConnectDpValue<T>(IDpValueSource<T> dpValue, TSourceConfiguration sourceConfiguration);

        void ConnectDpMethod(IDpActionSource dpMethod, TSourceConfiguration sourceConfiguration);
    }
}
