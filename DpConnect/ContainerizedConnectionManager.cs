using DpConnect;
using DpConnect.Connection;

using Promatis.Core;
using Promatis.Core.Extensions;
using Promatis.Core.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DpConnect
{
    public class ContainerizedConnectionManager : IDpConnectionManager
    {
        ILogger logger;
        IIoCContainer container;
        List<IDpConnection> connections = new List<IDpConnection>();

        public IEnumerable<IDpConnection> ConfiguredConnections => connections;

        public event EventHandler<IDpConnection> NewConnectionCreated;

        public ContainerizedConnectionManager(ILogger logger, IIoCContainer container)
        {
            this.logger = logger;
            this.container = container;
        }


        //public IDpConnection CreateConnection(IDpConnectionConfiguration configuration)
        //{            
        //    var method = typeof(IDpConnectionManager).GetMethods()
        //                    .Where(m => m.Name == nameof(IDpConnectionManager.CreateConnection) && m.IsGenericMethodDefinition)
        //                    .FirstOrDefault(m =>
        //                    {
        //                        var parameters = m.GetParameters();
        //                        return parameters.Length == 1 && parameters[0].ParameterType == typeof(IDpConnectionConfiguration);
        //                    });

        //    if (method != null)
        //    {
        //        var genericMethod = method.MakeGenericMethod(configuration.ConnectionType);
        //        object result = genericMethod.Invoke(this, new object[] { configuration });

        //        return (IDpConnection)result;
        //    }
        //    else
        //        throw new InvalidOperationException("Не найден обобщенный метод создания соединения");


        //public T CreateConnection<T, TConnectionConfig>(TConnectionConfig configuration) 
        //    where TConnectionConfig : IDpConnectionConfiguration
        //    where T : IDpConfigurableConnection<TConnectionConfig>

        //{
        //    T con = container.Resolve<T>();

        //    logger.Info($"Менеджер соединений: Создано новое подключение: {configuration.ConnectionId} с типом {con.GetType()}");
        //    con.Configure(configuration);


        //    connections.Add(con);

        //    NewConnectionCreated?.Invoke(this, con);

        //    return con;            

        //}
        public IDpConfigurableConnection<TConfig> CreateConnection<TConfig>(TConfig configuration) where TConfig : IDpConnectionConfiguration
        {

            IDpConfigurableConnection<TConfig> con = container.Resolve<IDpConfigurableConnection<TConfig>>(configuration.GetType());

            logger.Info($"Менеджер соединений: Создано новое подключение: {configuration.ConnectionId} с типом {con.GetType()}");
            con.Configure(configuration);


            connections.Add(con);

            NewConnectionCreated?.Invoke(this, con);

            return con;
        }

        public IDpConnection GetConnection(string Id)
        {
            return connections.First(c => c.Id == Id);
        }

        public void OpenConnections()
        {
            logger.Info("Открываем соединения...");
            connections.Where(c => c.Active).ForEach(c => c.Open());
            logger.Info("Соединения открыты.");
        }

        public void CloseConnections()
        {
            logger.Info("Закрываем соединения...");
            connections.Where(c => c.Active).ForEach(c => c.Close());
            logger.Info("Соединения закрыты.");
        }

        public IEnumerable<T> ResolveConnections<T>() where T : IDpConnection
        {
            IEnumerable<T> resolved = connections.Where(w => typeof(T).IsAssignableFrom(w.GetType())).Select(p => (T)p);

            if (resolved.Count() == 0)
                throw new InvalidOperationException($"В коллекции соединений не найден воркер с интерфейсом {typeof(T)}");
            else
                return resolved;
        }


    }
}
