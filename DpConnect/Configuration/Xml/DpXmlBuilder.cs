using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

using Promatis.Core.Logging;

using DpConnect.Building;
using System.Linq;
using DpConnect.Connection;

namespace DpConnect.Configuration.Xml
{
    public class DpXmlBuilder : IDpBuilder
    {
        readonly IDpConnectionManager connectionManager;
        readonly IDpWorkerManager workerManager;
        readonly IDpBinder dpBinder;
        readonly ILogger logger;
        
        public IDpWorkerManager WorkerManager { get { return workerManager; } }
        public IDpConnectionManager ConnectionManager { get { return connectionManager; } }

        readonly XDocument ConnectionConfiguration;
        readonly XDocument WorkerConfiguration;

        const string Xml_FileName = "DpConnect/DpConfig.xml";                

        const string Xml_DpConfigRootTag = "DpConfiguration";

        const string Xml_ConnectionRootTag = "Connections";
        const string Xml_ConnectionDeclareTag = "Connection";
        const string Xml_ConnectionTypeNameAttribute = "TypeName";
        const string Xml_ConnectionIdAttribute = "ConnectionId";
        const string Xml_ConnectionActiveAttribute = "Active";

        const string Xml_ConnectionIdTag = Xml_ConnectionIdAttribute;
        const string Xml_WorkerRootTag = "Workers";
        const string Xml_WorkerDeclareTag = "Worker";
        const string Xml_WorkerTypeNameAttribute = "TypeName";

        const string Xml_DpValueDeclareTag = "DpValue";
        const string Xml_DpValuePropNameAttribute = "PropertyName";
        const string Xml_DpSourceConfigurationTag = "SourceConfiguration";

        const string Xml_DpActionDeclareTag = "DpAction";


        public DpXmlBuilder(IDpConnectionManager connectionManager, IDpWorkerManager workerManager, ILogger logger, IDpBinder binder)
        {
            this.logger = logger;
            this.connectionManager = connectionManager;
            this.workerManager = workerManager;

            dpBinder = binder;


            logger.Info($"Загружаем конфигурацию {Xml_FileName}...");


            XDocument config = XDocument.Load(Xml_FileName);            

            ConnectionConfiguration = new XDocument(config.Element(Xml_DpConfigRootTag).Element(Xml_ConnectionRootTag));
            WorkerConfiguration = new XDocument(config.Element(Xml_DpConfigRootTag).Element(Xml_WorkerRootTag));
            
            logger.Info("Конфигурация завершена.");
        }

        public void Build()
        {
            CreateConnections();
            CreateWorkers();
        }

        void CreateConnections()
        {
            logger.Info("Создаём соединения...");
            
            //Пройдемся по всем элементом в xml, которые объявляют соединение
            foreach(XElement configuredConnection in ConnectionConfiguration.Root.Elements(Xml_ConnectionDeclareTag))
            {                
                //Нам важен тип соединения, который объявлен в xml. Именно такой тип мы и будем создавать
                string typeName = configuredConnection.Attribute(Xml_ConnectionTypeNameAttribute).Value;                                                
                Type connectionType = Type.GetType(typeName);

                //Соединение создается с аргументом - его конфигурация. Этот тип мы должны выяснить, чтобы создать эту конфигурацию.
                Type connectionConfigType = GetConnectionTypeGenericArg(connectionType, typeof(IDpConfigurableConnection<>));                
                
                //Здесь мы создаем этот тип, но нам нужен только его интерфейс, чтобы загрузкить xml конфиг
                IDpConnectionConfiguration connectionConfig = Activator.CreateInstance(connectionConfigType) as IDpConnectionConfiguration;
                connectionConfig.FromXml(new XDocument(configuredConnection));

                //Теперь вызываем метод у менеджера, который создаст соединение нужного типа и с нужной конфигурацией.
                //Этот метод обобщенный - он принимает тип соединения и тип конфигурации, чтобы законфигурировать соединение
                MethodInfo createConnectionMethodInfo = typeof(IDpConnectionManager).GetMethod(nameof(IDpConnectionManager.CreateConnection));
                MethodInfo createConnectionMethod = createConnectionMethodInfo.MakeGenericMethod(connectionConfigType);
                createConnectionMethod.Invoke(connectionManager, new object[] { connectionConfig });                
            }

            logger.Info("Соединения созданы.");
        }
        Type GetConnectionTypeGenericArg(Type connectionType, Type interfaceType)
        {
            Type iface = connectionType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType)
                .FirstOrDefault();
            if (iface == null)            
                throw new DpConfigurationException($"Тип соединения {connectionType} должен реализовывать интерфейс {interfaceType.Name}");             

            Type[] genericArguments = iface.GetGenericArguments();
            Type sourceConfigurationType = genericArguments[0];

            return sourceConfigurationType;
        }

        void CreateWorkers()
        {
            logger.Info("Создаём воркеров...");

            try
            {
                if(WorkerConfiguration.Root != null)
                {
                    IEnumerable<XElement> workers = WorkerConfiguration.Root.Elements(Xml_WorkerDeclareTag);
                    if (workers == null || workers.Count() == 0)
                        throw new DpConfigurationException("Нет воркеров в конфигурации");
                }
                else
                    throw new DpConfigurationException("Нет воркеров в конфигурации");
            }
            catch 
            {
                logger.Info("Нет воркеров в конфигурации.");
                return;
            }

            //Пройдемся по всем воркерам в xml
            foreach (XElement configuredWorker in WorkerConfiguration.Root.Elements(Xml_WorkerDeclareTag))
            {
                //Создадим воркера с указанным в xml типе
                string typeName = configuredWorker.Attribute(Xml_WorkerTypeNameAttribute).Value;
                Type workerType = Type.GetType(typeName);
                IDpWorker worker = CreateWorker(workerType);
                
                //Получим соединение, к которому привязан воркер
                string connectionId = configuredWorker.Element(Xml_ConnectionIdTag).Value;
                IDpConnection connection = connectionManager.GetConnection(connectionId);
                                
                //Теперь мы должны взять тип конфигурации точки и создать эту конфигурацию.
                //Для этого, соединение должна реализовывать интерфейс IDpBindableConnection<T>, где T - тип конфигурации.
                Type connectionType = connection.GetType();
                Type sourceConfigurationType = GetConnectionTypeGenericArg(connectionType, typeof(IDpBindableConnection<>));

                //Создадим конфигурацию нужного типа
                Type genericDpConfigType = typeof(DpConfiguration<>).MakeGenericType(sourceConfigurationType);
                                
                // Создаём список для конфигураций
                var listType = typeof(List<>).MakeGenericType(genericDpConfigType);
                var listOfConfigs = Activator.CreateInstance(listType);

                var addMethod = listType.GetMethod("Add");

                //Пройдемся по всем конфигурациям в xml и добавим их в список
                List<XElement> listOfXmlConfigs = new List<XElement>();
                listOfXmlConfigs.AddRange(configuredWorker.Elements(Xml_DpValueDeclareTag));
                listOfXmlConfigs.AddRange(configuredWorker.Elements(Xml_DpActionDeclareTag));

                foreach (XElement configuredDp in listOfXmlConfigs)
                {
                    //Создадим конфигурацию нужного типа
                    IDpSourceConfiguration sourceConfiguration = Activator.CreateInstance(sourceConfigurationType) as IDpSourceConfiguration;                                            
                    IDpConfiguration dpConfig = Activator.CreateInstance(genericDpConfigType) as IDpConfiguration;

                    dpConfig.PropertyName = configuredDp.Attribute(Xml_DpValuePropNameAttribute).Value;
                    sourceConfiguration.FromXml(new XDocument(configuredDp.Element(Xml_DpSourceConfigurationTag)));
                    dpConfig.SourceConfiguration = sourceConfiguration;

                    addMethod.Invoke(listOfConfigs, new[] { dpConfig });
                }

                //Так как метод Bind - Обобщенный, он принимает нужный тип конфигурации, вызовим его через рефлекшн
                MethodInfo bindMethodInfo = typeof(IDpBinder).GetMethod(nameof(IDpBinder.Bind));
                MethodInfo bindMethod = bindMethodInfo.MakeGenericMethod(sourceConfigurationType);

                bindMethod.Invoke(dpBinder, new object[] { worker, connection, listOfConfigs });

            }
            logger.Info("Воркеры созданы.");
        }

        IDpWorker CreateWorker(Type workerType)
        {
            if (!typeof(IDpWorker).IsAssignableFrom(workerType))
                throw new DpConfigurationException($"Тип {workerType} должен реализовывать интерфейс {nameof(IDpWorker)}");

            MethodInfo createWorkerMethodInfo = typeof(IDpWorkerManager).GetMethod(nameof(IDpWorkerManager.CreateWorker));
            MethodInfo createWorkerMethod = createWorkerMethodInfo.MakeGenericMethod(workerType);

            return (IDpWorker)createWorkerMethod.Invoke(workerManager, null);
        }
        

    }
}
