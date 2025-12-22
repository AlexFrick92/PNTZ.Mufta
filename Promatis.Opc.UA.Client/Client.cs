using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Client.ComplexTypes;
using Promatis.Core.Logging;


namespace Promatis.Opc.UA.Client
{
    /// <summary> 
    /// OPC UA Client. Фасад для Foundation OPC UA
    /// создаёт соединение с сервером и поддерживает его.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Переподключение завершено
        /// </summary>
        public event EventHandler ReconnectComplete;

        /// <summary>
        /// Запущено переподключение
        /// </summary>
        public event EventHandler ReconnectStarting;

        /// <summary>
        /// Соединение установлено
        /// </summary>
        public event EventHandler ConnectComplete;

        public bool IsConnected { get; set; } = false;

        const int ReconnectPeriod = 10;
        SessionReconnectHandler _reconnectHandler;
        private string _endPoint;
        Session _session;
        private Subscription _subscription;

        private ILogger _logger;
        private string _username;
        private string _password;

        /// <summary>
        /// Использовуется как интервал опроса ноды подписки мс
        /// </summary>
        public int PublishingInterval { get; set; } = 10;

        //
        public Client(string endPoint, ILogger logger)
        {
            _endPoint = endPoint;
            _logger = logger;

            //Utils.SetTraceLog("C:\\Source\\Promatis\\Tools\\OpcClient\\Source\\Opc.Ua.Client\\ConsoleClient\\bin\\Debug\\OpcTrace.log", true);
            //Utils.SetTraceMask(0x3ff);
            // Utils.SetLogLevel(LogLevel.Trace);
            //Utils.SetTraceOutput(Utils.TraceOutput.DebugAndFile);
            //var a = Utils.Tracing;
            /*
    private const int TraceId = 1;
    private const int DebugId = 2;
    private const int InfoId = 3;
    private const int WarningId = 4;
    private const int ErrorId = 5;
    private const int CriticalId = 6;
    /// <summary>The core event ids.</summary>
    private const int ServiceCallStartId = 9;
    private const int ServiceCallStopId = 10;
    private const int ServiceCallBadStopId = 11;
    private const int SubscriptionStateId = 12;
    private const int SendResponseId = 13;
    private const int ServiceFaultId = 14;
             */
            //          Utils.SetTraceMask();
            //          Utils.SetTraceOutput( Utils.TraceOutput.FileOnly);
        }

        public Client(string endPoint, ILogger logger, string username, string password) : this(endPoint, logger)
        {
            _username = username;
            _password = password;

            ConnectComplete += (s, e) => IsConnected = true;
            ReconnectStarting += (s, e) => IsConnected = false;
            ReconnectComplete += (s, e) => IsConnected = false; 
        }

        private void FastEventCallback(Subscription subscription, EventNotificationList notification,
            IList<string> stringtable)
        {
            _logger.Info($"подписка:");
        }

        private void Client_KeepAlive(Session session, KeepAliveEventArgs e)
        {
            try
            {
                // check for events from discarded sessions.
                if (!ReferenceEquals(_session, session)) return;

                if (!ServiceResult.IsBad(e.Status)) return;

                if (_reconnectHandler == null)
                {
                    ReconnectStarting?.Invoke(this, EventArgs.Empty);
                    _logger.Info("--- Переподключение ---");
                    _reconnectHandler = new SessionReconnectHandler(true);
                    _reconnectHandler.BeginReconnect(session, ReconnectPeriod * 1000, Client_ReconnectComplete);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
            }
        }

        private void Client_ReconnectComplete(object sender, EventArgs e)
        {
            // ignore callbacks from discarded objects.
            if (!ReferenceEquals(sender, _reconnectHandler)) return;
            if (_reconnectHandler.Session != null)
                _session = _reconnectHandler.Session;
            _reconnectHandler.Dispose();
            _reconnectHandler = null;
            ReconnectComplete?.Invoke(this, EventArgs.Empty);
            _logger.Info("--- Переподключение выполнено ---");
        }

        /// <summary>
        /// Получение информации о хосте
        /// </summary>
        /// <returns>Полной адрес хоста</returns>
        public string GetHost() => _endPoint;

        /// <summary>
        /// Создание клиента для ассинхронной задачи. Клиент ожидает соединения с сервером
        /// </summary>
        /// <returns></returns>
        public async Task CreateClient()
        {

            
            var config = new ApplicationConfiguration
            {
                ApplicationName = "Atissa Client",
                ApplicationType = ApplicationType.Client,

                TransportQuotas = new TransportQuotas()
                {
                    OperationTimeout = 25000,
                    MaxArrayLength = 1048576,
                    MaxByteStringLength = 4194304,
                    MaxMessageSize = 4194304,
                    MaxBufferSize = 65535,
                    ChannelLifetime = 5000,
                    SecurityTokenLifetime = 3600000,
                },
                /*
                TraceConfiguration = new TraceConfiguration()
                {
                    OutputFilePath = "C:\\Source\\Promatis\\Tools\\OpcClient\\Source\\Opc.Ua.Client\\ConsoleClient\\bin\\Debug\\OpcTrace.log" ,
                    DeleteOnLoad = true,
                    TraceMasks = 255,
                },
                */
                SecurityConfiguration = new SecurityConfiguration() { AutoAcceptUntrustedCertificates = true },
                ClientConfiguration = new ClientConfiguration()
                {
                    DefaultSessionTimeout = 10000,
                    WellKnownDiscoveryUrls =
                    {
                        "opc.tcp://{0}:4840",
                        "opc.tcp://{0}:4840/UADiscovery",
                        "http://{0}/UADiscovery/Default.svc"
                    },
                    MinSubscriptionLifetime = 10000,

                },
                DisableHiResClock = true,
            };

            var endpointConfiguration = EndpointConfiguration.Create(config);
            EndpointDescription selectedEndpoint  = CoreClientUtils.SelectEndpoint(_endPoint, false, 10000);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
            IUserIdentity userIdentity = null;
            if (!string.IsNullOrEmpty(_username))
            {
                userIdentity = new UserIdentity(_username, _password);
            }

            _session = await Session.Create(config, endpoint, false, "Atissa", 10000, userIdentity, null);

            var typeSystemLoader = new ComplexTypeSystem(_session);
            await typeSystemLoader.Load();
            _session.KeepAliveInterval = 2000;
            _session.KeepAlive += Client_KeepAlive;

            _subscription = new Subscription(_session.DefaultSubscription)
            {
                PublishingEnabled = true,
                PublishingInterval = PublishingInterval,
                DisplayName = "Subscription for ATISSA",
            };
            _session.AddSubscription(_subscription);
            await _subscription.CreateAsync();
            ConnectComplete?.Invoke(this, null);
        }

        public MonitoredItem Subscription<T>(NodeValue<T> node)
        {
            if (_session == null) return null;
            var item = new MonitoredItem(_subscription.DefaultItem)
            {
                StartNodeId = node.NodeId,
                DisplayName = node.NodeId.ToString(),

                //AKF
                SamplingInterval = 10,
                QueueSize = 10,
            };
            _logger.Info($"Добавлена подписка на: {item.DisplayName}");
            item.Notification += node.ItemNotification;
            _subscription.AddItem(item);
            _subscription.ApplyChanges();

            return item;
        }

        public void Unsubscription(MonitoredItem item)
        {
            if (_session == null) return;
            _subscription.RemoveItem(item);
            _subscription.ApplyChanges();
            _logger.Info($"Отменена подписка на: {item.DisplayName}");
        }

        public void SubscriptionEvent<T>(NodeValue<T> node)
        {
            var item = new MonitoredItem(_subscription.DefaultItem)
            {
                NodeClass = NodeClass.Object,
                AttributeId = Attributes.EventNotifier,
                StartNodeId = new NodeId("ns=3;i=1845"),
                DisplayName = node.NodeId.ToString(),
            };
            item.Notification += node.ItemNotification;
            _subscription.AddItem(item);
            _subscription.ApplyChanges();
        }

        /// <summary>
        /// прочитать значение ноды
        /// </summary>
        /// <typeparam name="T">тип значения. Если тип не элементарный он должен поддеривать интерфейс <see cref="INodeConverter"/></typeparam>
        /// <param name="node">Нода для чтения</param>
        /// <returns>значение ноды</returns>
        public T GetNodeValue<T>(NodeValue<T> node)
        {
            if (_session == null) return default;

            var n = _session.ReadNode(node.NodeId);
            var value = _session.ReadValue(node.NodeId);
            node.FromNode(value);
            return node.Value;
        }

        public T Test<T>(NodeValue<T> node)
        {
            if (_session == null) return default;
            var nodesToRead = new ReadValueIdCollection();

            var attributeId = Atributes.GetIdentifier("Value");
            var nodeToRead = new ReadValueId()
            {
                NodeId = node.NodeId,
                AttributeId = attributeId,
            };
            nodesToRead.Add(nodeToRead);
            var n = _session.ReadNode(node.NodeId);
            var value = _session.Read(null, 0, TimestampsToReturn.Neither, nodesToRead, out var results,
                out var diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToRead);

            return node.Value;
        }

        //            _session.Va(v);

        /// <summary>
        ///  Сохранение значения ноды на сервере
        /// </summary>
        /// <typeparam name="T">Тип значения ноды</typeparam>
        /// <param name="node">Нода с новым значением в Value</param>
        /// <returns>Код ошибки при записи <see cref="StatusCode"/> Исполользуйте <code>StatusCode.IsBad</code> для проверки</returns>
        public StatusCode ModifyNodeValue<T>(NodeValue<T> node)
        {
            if (_session == null) return default;
            var nodesToWrite = new WriteValueCollection();
            var n = _session.ReadValue(node.NodeId);
            var nodeToWrite = new WriteValue { NodeId = node.NodeId, AttributeId = Attributes.Value };
            nodesToWrite.Add(nodeToWrite);

            nodeToWrite.Value.WrappedValue = new Variant(node.Value, n.WrappedValue.TypeInfo);
            var requestHeader = new RequestHeader
            {
                ReturnDiagnostics = (uint)DiagnosticsMasks.All
            };

            var responseHeader = _session.Write(
                requestHeader,
                nodesToWrite,
                out var results,
                out var diagnosticInfos);
            ClientBase.ValidateResponse(results, nodesToWrite);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToWrite);
            return results[0];
/*            if (StatusCode.IsBad(results[0]))
            {
                Console.WriteLine($"{results[0]} {diagnosticInfos} {responseHeader.StringTable}");
            }
*/
        }

        public StatusCode ModifyNodeComplexValue<T>(NodeValue<ComplexType<T>> node)
        {
            if (_session == null) return default;
            var nodesToWrite = new WriteValueCollection();
            var n = _session.ReadValue(node.NodeId);
            var nodeToWrite = new WriteValue { NodeId = node.NodeId, AttributeId = Attributes.Value };
            nodesToWrite.Add(nodeToWrite);

            nodeToWrite.Value.WrappedValue = new Variant(node.Value.Value, n.WrappedValue.TypeInfo);
            var requestHeader = new RequestHeader
            {
                ReturnDiagnostics = (uint)DiagnosticsMasks.All
            };

            var responseHeader = _session.Write(
                requestHeader,
                nodesToWrite,
                out var results,
                out var diagnosticInfos);
            ClientBase.ValidateResponse(results, nodesToWrite);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToWrite);
            return results[0];
        }

        /// <summary>
        ///  Сохранение значения ноды на сервере
        /// </summary>
        /// <typeparam name="T">Тип значения ноды</typeparam>
        /// <param name="nodes">Список нод с новыми значениями в Value</param>
        /// <returns>Код ошибок при записи <inheritdoc cref="StatusCodeCollection"/> Исполользуйте <code>StatusCode.IsBad</code> для проверки</returns>
        public StatusCodeCollection ModifyNodeValue<T>(IEnumerable<NodeValue<T>> nodes)
        {
            if (_session == null) return default;
            var nodesToWrite = new WriteValueCollection();
            foreach (var node in nodes)
            {
                var n = _session.ReadValue(node.NodeId);
                var nodeToWrite = new WriteValue { NodeId = node.NodeId, AttributeId = Attributes.Value };
                nodesToWrite.Add(nodeToWrite);
                nodeToWrite.Value.WrappedValue = new Variant(node.Value, n.WrappedValue.TypeInfo);
            }

            var requestHeader = new RequestHeader
            {
                ReturnDiagnostics = (uint)DiagnosticsMasks.All
            };


            var responseHeader = _session.Write(
                requestHeader,
                nodesToWrite,
                out var results,
                out var diagnosticInfos);
            ClientBase.ValidateResponse(results, nodesToWrite);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToWrite);
            return results;
/*            if (StatusCode.IsBad(results[0]))
            {
                Console.WriteLine($"{results[0]} {diagnosticInfos} {responseHeader.StringTable}");
            }
*/
        }

        public T CallMethod<T>(NodeFunc<T> node, params object[] args)
        {
            try
            {

                if (_session == null) return default;

                node.FromNode(_session.Call(node.NodeId, node.NodeMethodId, args));
                return node.Value;
            }
            catch (Exception e)
            {
                _logger.Error(
                    $"[CallMethod<T>] {e.Message} {e.InnerException} {e.StackTrace}. NodeId:{node?.NodeId} NodeMethodId:{node.NodeMethodId} args.Length:{args?.Length}");
            }

            return default;
        }

        public IList<object> CallMethod(string nodePath, params object[] args)
        {
            if (_session == null)
            {
                _logger.Error($"Opc.UA.Client _session есть null!");
                return null;
            }

            try
            {
                return _session.Call(new NodeId(nodePath), new NodeId($"{nodePath}.Method"), args);
            }
            catch (Exception e)
            {
                _logger.Error(
                    $"[CallMethod] {e.Message} {e.InnerException} {e.StackTrace} nodePath:{nodePath}, NodeMethod:{($"{nodePath}.Method")}, args.Length:{args?.Length}, args[0]:{args?[0]}");
                return null;
            }
        }
        public IList<object> CallPrimitiveMethod(string nodePath, params object[] args)
        {
            if (_session == null)
            {
                _logger.Error($"Opc.UA.Client _session есть null!");
                return null;
            }

            try
            {
                return _session.Call(new NodeId(nodePath), new NodeId(nodePath), args);
            }
            catch (Exception e)
            {
                _logger.Error(
                    $"[CallMethod] {e.Message} {e.InnerException} {e.StackTrace} nodePath:{nodePath}, NodeMethod:{($"{nodePath}.Method")}, args.Length:{args?.Length}, args[0]:{args?[0]}");
                return null;
            }
        }

        /// <summary>
        /// Вызов метода OPC, который возвращает комплексный тип в формате Siemens
        /// </summary>
        /// <typeparam name="T">Класс, описывающий результат вызова</typeparam>
        /// <param name="method">Название метода (название ноды с методом)</param>
        /// <param name="inputArguments">Список входных параметров</param>
        /// <returns></returns>
        public T CallMethod<T>(string method, VariantCollection inputArguments) where T : class
        {
            var objectId = new NodeId(method);
            var metodId = new NodeId(method + ".Method");
            var OutputArguments = GetInputOutputArguments(metodId, false);
            var result = CallValue(objectId, metodId, inputArguments, OutputArguments);
            return (GetValueT(typeof(T), OutputArguments, result[0].OutputArguments, string.Empty) as T);
        }
        /// <summary>
        /// Вызов метода OPC, который возвращает комплексный тип в универсальном формате
        /// </summary>
        /// <typeparam name="T">Класс, описывающий результат вызова</typeparam>
        /// <param name="parent">Родительская нода для вызываемого метода</param>
        /// <param name="method"Название метода (название ноды)</param>
        /// <param name="inputArguments">Список входных параметров</param>
        /// <returns></returns>
        public T CallMethod<T>(string parent, string method, VariantCollection inputArguments) where T : class, new()
        {
            var objectId = new NodeId(parent);
            var metodId = new NodeId(method);
            var OutputArguments = GetInputOutputArguments(metodId, false);
            var result = CallValue(objectId, metodId, inputArguments, OutputArguments);
            return (GetValueT(typeof(T), OutputArguments, result[0].OutputArguments, string.Empty) as T);
        }

        public VariantCollection GetInputOutputArguments(NodeId methodId, bool inputArgs)
            {
                if (_session == null) throw new ArgumentNullException("session");
                if (methodId == null) throw new ArgumentNullException("methodId");

                var method = _session.NodeCache.Find(methodId) as MethodNode;

                if (method == null) return null;
                var browseName = inputArgs ? BrowseNames.InputArguments : BrowseNames.OutputArguments;

                // fetch the argument list.
                var argumentsNode =
                    _session.NodeCache.Find(methodId, ReferenceTypeIds.HasProperty, false, true, browseName) as
                        VariableNode;

                if (argumentsNode == null) return null;
                
                //Получаем типы и название переменных
                var value = _session.ReadValue(argumentsNode.NodeId);
                var argumentsList = value.Value as ExtensionObject[];
                var arguments = new VariantCollection();
                if (argumentsList == null) return arguments;
                for (int ii = 0; ii < argumentsList.Length; ii++)
                {
                    var argument = argumentsList[ii].Body as Argument;
                    if (argument != null) arguments.Add(new Variant(argument));
                }
                return arguments;
            }

        private List<CallMethodResult> CallValue(NodeId objectId, NodeId methodId, VariantCollection inputArguments,
            VariantCollection outputArguments)
        {
            try
            {
                CallMethodRequest request = new CallMethodRequest();

                request.ObjectId = objectId;
                request.MethodId = methodId;
                request.InputArguments = inputArguments;

                CallMethodRequestCollection requests = new CallMethodRequestCollection();
                requests.Add(request);

                CallMethodResultCollection results;
                DiagnosticInfoCollection diagnosticInfos;

                ResponseHeader responseHeader = _session.Call(
                    null,
                    requests,
                    out results,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(results, requests);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, requests);

                if (StatusCode.IsBad(results[0].StatusCode))
                {
                    throw new ServiceResultException(new ServiceResult(results[0].StatusCode, 0, diagnosticInfos,
                        responseHeader.StringTable));
                }

                return results.ToList();
            }
            catch (Exception exception)
            {
                //GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">Тип результата</param>
        /// <param name="outputArguments"></param>
        /// <param name="value">результат</param>
        /// <param name="path"></param>
        /// <returns></returns>
        private object GetValueT(Type type, VariantCollection outputArguments, object value, string path)
        {
            var result = Activator.CreateInstance(type);
            if (value == null) return result;

            var variant = value as Variant?;
            if (variant != null)
            {
                var property = GetProperty(result, path);
                if (property != null) property.SetValue(value, variant.Value.Value);
            }

            switch (value)
            {
                case ExtensionObject extension:
                    result = GetValueT(type, outputArguments, extension.Body, path);
                    break;
                case IEncodeable encodeable:
                    var properties = encodeable.GetType().GetProperties();
                    foreach (var propertyInfo in properties)
                    {
                        var name = Utils.GetDataMemberName(propertyInfo);
                        var property = GetProperty(result, name);
                        if (property == null) continue;
                        if (property.PropertyType.IsPrimitive || property.PropertyType.Name ==nameof(DateTime))
                            property.SetValue(result, propertyInfo.GetValue(value));
                        else
                            property.SetValue(result,
                                GetValueT(property.PropertyType, outputArguments, propertyInfo.GetValue(value),
                                    string.Empty));
                    }

                    break;
                case VariantCollection variantColection:
                {
                    for (int i = 0; i < variantColection.Count; i++)
                    {
                        var extensionObject = outputArguments[i].Value as ExtensionObject;
                        var arg = extensionObject.Body as Argument;
                        var property = GetProperty(result, arg.Name);
                        if (property == null) continue;
                        if (property.PropertyType.IsValueType) property.SetValue(result, variantColection[i].Value);
                        if (property.PropertyType.IsClass)
                        {
                            property.SetValue(result,
                                GetValueT(property.PropertyType, outputArguments, variantColection[i].Value,
                                    arg.Name));
                        }
                    }
                    break;
                }
                case IEnumerable enumerables:
                    var r = result as IList;
                    if (r == null) break;
                    foreach (var enumerable in enumerables)
                        r.Add(GetValueT(type.GenericTypeArguments[0], outputArguments, enumerable, string.Empty));
                    break;
            }
            return result;
        }
        /// <summary>
        ///  получить свойство по имени OPC
        /// </summary>
        /// <param name="result">Свойство</param>
        /// <param name="name">Имя OPC</param>
        /// <returns></returns>
        private PropertyInfo GetProperty(object result, string name)
        {
            PropertyInfo[] properties = result.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                var attr = propertyInfo.GetCustomAttribute(typeof(OpcDataAttribute)) as OpcDataAttribute;
                if (attr == null) continue;
                if (attr.Name != name) continue;
                return propertyInfo;
            }

            return null;
        }


        private void OnDataChangeNotification(Subscription subscription, DataChangeNotification notification,
            IList<string> stringtable)
        {
//            Console.WriteLine(notification.TypeId.ToString());
//            Console.WriteLine("--- OnDataChangeNotification ---");
        }

        public void Start()
        {
            if (_session == null)
            {                

                try
                {
                    CreateClient().Wait();
                }
                catch (AggregateException ex)
                {
                    Exception innerEx = ex.InnerException;
                    string message = ex.Message;
                    while (innerEx != null)
                    {
                        message += " - " + innerEx.Message;
                        innerEx = innerEx.InnerException;
                    }

                    throw new Exception(message);
                }
                catch (Exception ex)
                {
                    Utils.Trace("ServiceResultException:" + ex.Message);
                    _logger.Info("Exception: {0}", ex.Message);
                    throw;
                }
            }
            else
            {
                throw new Exception("Клиент уже запущен!");
            }
            
        }

        public void Stop()
        {
            if (_session != null)
            {
                _session.Close();
                if (_session.KeepAliveStopped)
                {
                    //               exitCode = ExitCode.ErrorNoKeepAlive;
                    return;
                }
                _session = null;
            }
        }

    }

}
