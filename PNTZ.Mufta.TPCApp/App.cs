using Desktop.Application;


using DpConnect;
using DpConnect.Configuration.Xml;
using DpConnect.Connection;
using DpConnect.OpcUa;


using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.View;


using Promatis.Core;
using Promatis.Core.Logging;
using Promatis.IoC.DryIoc;


namespace PNTZ.Mufta.TPCApp
{
    internal class App : StagedApplication
    {

        public IDpBuilder DpBuilder { get; private set; }
        public IDpConnectionManager DpConnectionManager { get => DpBuilder.ConnectionManager; }
        public IDpWorkerManager DpWorkerManager { get => DpBuilder.WorkerManager; }



        protected override void BeforeInit()
        {
            IIoCContainer container = new DryIocContainer();

            container.RegisterInstance(container);

            container.RegisterInstance<ILogger>(new ConsoleLogger());

            container.Register<IOpcUaConnection, OpcUaConnection>();
            container.Register<IDpConnectionManager, ContainerizedConnectionManager>();

            container.Register<IMakeHeartBeat, MakeHeartBeat>();
            container.Register<IDpWorkerManager, ContainerizedWorkerManager>();

            container.Register<IDpBuilder, DpXmlBuilder>();

            DpBuilder = container.Resolve<IDpBuilder>();            

            DpBuilder.Build();
        }

        protected override void Init()
        {
            mainWindow = new MainView();

            DpConnectionManager.OpenConnections();
        }

        protected override void AfterInit() 
        {
            
        }
    }
}
