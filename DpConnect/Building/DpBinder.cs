using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DpConnect.Configuration;
using DpConnect.Connection;
using Promatis.Core.Logging;

namespace DpConnect.Building
{
    public class DpBinder : IDpBinder
    {

        readonly ILogger logger;


        public DpBinder(ILogger logger)
        {
            this.logger = logger;
        }


        //Создать точку такого типа, который имеет свойство воркера
        public void Bind<TSourceConfig>(IDpWorker worker, IDpBindableConnection<TSourceConfig> connection, IEnumerable<DpConfiguration<TSourceConfig>> configs)
            where TSourceConfig : IDpSourceConfiguration
        {
            logger.Info($"Связываем {worker.GetType()}...");
            foreach (var config in configs)
            {
                PropertyInfo prop = null;
                try
                {
                    prop = worker.GetType().GetProperties().First(p => p.Name == config.PropertyName);
                }
                catch (InvalidOperationException ex)
                {
                    throw new DpConfigurationException($"В типе {worker.GetType()} не найдено публичное свойство {config.PropertyName}");
                }                               

                if (prop.PropertyType.GetGenericTypeDefinition() == typeof(IDpValue<>))
                {
                    object dp = CreateDpValue(prop);
                    connection.ConnectDpValue(dp as dynamic, config.SourceConfiguration);
                    prop.SetValue(worker, dp);
                    logger.Info($"Свойство {config.PropertyName} типа {dp.GetType()} для {config.ConnectionId}");

                }
                else if (prop.PropertyType.GetGenericTypeDefinition() == typeof(IDpAction<>))
                {
                    object dp = CreatDpFunc(prop);
                    connection.ConnectDpMethod(dp as dynamic, config.SourceConfiguration);
                    prop.SetValue(worker, dp);
                    logger.Info($"Метод {config.PropertyName} типа {dp.GetType()} для {config.ConnectionId}");
                }
                else
                {
                    string messageError = $"Ошибка при связывании воркера. Свойство {prop.Name} : {prop.PropertyType} воркера должно быть одним из следующих типов: {typeof(IDpValue<>)}, {typeof(IDpAction<>)}";
                    logger.Error(messageError);
                    throw new DpConfigurationException(messageError);
                }                                    
            }

            CheckForUnboundProps(worker);

            worker.DpBound();
            logger.Info($"{worker.GetType()} связан.");
        }


        void CheckForUnboundProps(IDpWorker worker)
        {
            //проверить, если остались непривязанные свойства

            List<PropertyInfo> unboundProps = new List<PropertyInfo>();


            foreach (var prop in worker.GetType().GetProperties().Where(p => p.PropertyType.IsGenericType).Where(p =>
                p.PropertyType.GetGenericTypeDefinition() == typeof(IDpAction<>)
                || p.PropertyType.GetGenericTypeDefinition() == typeof(IDpValue<>)
            ))
            {
                if (prop.GetValue(worker) is null)
                    unboundProps.Add(prop);
            }
            if (unboundProps.Count() > 0)
            {
                logger.Info($"Следующие свойства для {worker.GetType()} не были привязаны из за отсутствия конфигурации:");
                foreach (var prop in unboundProps)
                {
                    logger.Info(prop.Name);
                }
                throw new DpConfigurationException($"Для {worker.GetType()} остались непривязанные свойства..");
            }
        }

        object CreateDpValue(PropertyInfo property)
        {
            Type[] propGenericType = property.PropertyType.GetGenericArguments();

            if(!propGenericType[0].IsVisible)
            {
                throw new DpConfigurationException($"Класс {propGenericType[0]} указанный при объявлении IDpValue<>, должен быть public");
            }            

            Type genericType = typeof(DpValue<>).MakeGenericType(propGenericType);
            return Activator.CreateInstance(genericType);
        }
        object CreatDpFunc(PropertyInfo property)
        {
            Type[] propGenericType = property.PropertyType.GetGenericArguments();
            Type genericType = typeof(DpAction<>).MakeGenericType(propGenericType);            
            
            return Activator.CreateInstance(genericType);


        }
    }
}
