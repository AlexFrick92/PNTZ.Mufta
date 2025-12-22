
using System.Collections.Generic;

using DpConnect;
using DpConnect.Configuration;
using DpConnect.Connection;

namespace DpConnect.Building
{
    public interface IDpBinder
    {
        void Bind<TSourceConfig>(IDpWorker worker, IDpBindableConnection<TSourceConfig> connectionToBind, IEnumerable<DpConfiguration<TSourceConfig>> configs)
            where TSourceConfig : IDpSourceConfiguration;            

    }
}
