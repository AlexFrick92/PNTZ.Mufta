

using System;
using System.Collections.Generic;

namespace DpConnect
{
    /// <summary>
    /// Интерфейс, через который можно получить коллекцию воркеров
    /// </summary>

    public interface IDpWorkerManager
    {
        T CreateWorker<T>() where T : IDpWorker;
        

        /// <summary>
        /// Получить список воркеров указанного типа
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> ResolveWorker<T>() where T : IDpWorker;

        /// <summary>
        /// Создан новый воркер
        /// </summary>
        event EventHandler<IDpWorker> WorkerCreated;
        
    }
}
