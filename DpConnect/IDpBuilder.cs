

using DpConnect.Configuration;
using System.Collections;
using System.Collections.Generic;

namespace DpConnect
{
    public interface IDpBuilder
    {
        void Build();

        IDpConnectionManager ConnectionManager { get; }
        IDpWorkerManager WorkerManager { get; }

       // IDpWorker BuildWorker<T>(IEnumerable<DpConfiguration> propertyConfiguration) where T : IDpWorker;

    }
}
