using System;

using System.Collections.Generic;

using Promatis.Opc.UA.Client;
using Promatis.Core.Logging;

using DpConnect.Connection;

using DpConnect.Configuration.Xml;
using DpConnect.Exceptions;

namespace DpConnect.OpcUa
{
    public class OpcUaConnection : IOpcUaConnection
    {
        Client client;
        ILogger logger;
        OpcUaConnectionConfiguration connectionConfiguration;

        IList<object> nodes = new List<object>();

        IList<IDpStatus> dpValuesStatus = new List<IDpStatus>();

        public string Id { get; private set; }

        public bool Active { get; private set; }

        public OpcUaConnection(ILogger logger)
        {
            this.logger = logger;
        }

        public void Configure(OpcUaConnectionConfiguration configuration)
        {
            Id = configuration.ConnectionId;
            Active = configuration.Active;

            connectionConfiguration = configuration;

            logger.Info($"{nameof(OpcUaConnection)}: Соединение {connectionConfiguration.ConnectionId} законфигурировано: {connectionConfiguration.Endpoint}");
        }
        
        public void Open()
        {
            logger.Info($"{Id}: Запуск...");

            if (client != null && client.IsConnected)
            {
                logger.Info("Клиент уже запущен!");
                return;
            }

            try
            {
                logger.Info("Подключение к " + connectionConfiguration.Endpoint);
                if (client == null)
                    client = new Client(connectionConfiguration.Endpoint, logger);

                client.Start();

                logger.Info($"{Id}: Запустился!, конфигурируем точки...");

                foreach (var node in nodes)
                {
                    client.Subscription(node as dynamic);                    
                }
                foreach (var status in dpValuesStatus)
                {
                    status.IsConnected = true;
                }
                logger.Info($"{Id}: Точки законфигурированы.");

            }
            catch (Exception ex)
            {
                logger.Info($"{Id}: Не удалось запустить! {ex.Message}");
                throw;
            }

        }
        public void Close()
        {
            client?.Stop();
            logger.Info($"{Id}: Остановился");

            foreach(var status in dpValuesStatus)
            {
                status.IsConnected = false;
            }
        }

        public void ConnectDpValue<T>(IDpValueSource<T> dpValue, OpcUaDpValueSourceConfiguration sourceConfiguration)
        {
            if (typeof(T).IsClass && !(typeof(T) == typeof(string)))
                nodes.Add(ConfigureNodeComplexValue(dpValue, sourceConfiguration));
            else
                nodes.Add(ConfigureNodeValue(dpValue, sourceConfiguration));

            logger.Info($"{Id}: Зарегистрирована точка {sourceConfiguration.NodeId}");
        }

        NodeValue<T> ConfigureNodeValue<T>(IDpValueSource<T> dpValue, OpcUaDpValueSourceConfiguration config)
        {
            NodeValue<T> node = new NodeValue<T>(config.NodeId, (e, v) => dpValue.UpdateValueFromSource(v));

            dpValue.ValueWritten += (e, v) =>
            {
                node.Value = v;
                if (client != null)
                {
                    try
                    {
                        client.ModifyNodeValue(node);                    
                    }
                    catch(Exception ex)
                    {                     
                        throw new TransportLevelDpException($"Не удалось записать ноду {config.NodeId}", ex);
                    }
                }
                else
                {
                    throw new Exception($"{Id}: Подключение сервером не установлено!");
                }
            };

            dpValuesStatus.Add(dpValue);
            return node;
        }

        NodeValue<ComplexType<T>> ConfigureNodeComplexValue<T>(IDpValueSource<T> dpValue, OpcUaDpValueSourceConfiguration config)
        {
            NodeValue<ComplexType<T>> node = new NodeValue<ComplexType<T>>(config.NodeId, (e, v) => dpValue.UpdateValueFromSource(v.ExtractedValue));

            dpValue.ValueWritten += (e, v) =>
            {                
                node.Value.ExtractedValue = v;
                client.ModifyNodeComplexValue(node);                
            };
            dpValuesStatus.Add(dpValue);

            dpValue.UpdateValueFromSource((T)Activator.CreateInstance(typeof(T))) ;
            return node;
        }

        public void ConnectDpMethod(IDpActionSource dpMethod, OpcUaDpValueSourceConfiguration sourceConfiguration)
        {

            dpMethod.SourceDelegate += (e) =>
            {
                if (client != null)
                {

                    return client.CallMethod(sourceConfiguration.NodeId, e);
                }
                else
                {
                    throw new Exception($"{Id}: Подключение сервером не установлено!");
                }
            };
            dpValuesStatus.Add(dpMethod);
        }
    }
}
