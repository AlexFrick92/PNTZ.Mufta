using Desktop.Application;


using DpConnect;
using DpConnect.Configuration.Xml;
using DpConnect.Connection;
using DpConnect.OpcUa;


using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.View;
using PNTZ.Mufta.TPCApp.ViewModel;
using Promatis.Core;
using Promatis.Core.Logging;
using Promatis.IoC.DryIoc;
using System;
using System.Linq;
using Toolkit.IO;
using Toolkit.Logging;


namespace PNTZ.Mufta.TPCApp
{
    internal class App : StagedApplication
    {

        public IDpBuilder DpBuilder { get; private set; }
        public IDpConnectionManager DpConnectionManager { get => DpBuilder.ConnectionManager; }
        public IDpWorkerManager DpWorkerManager { get => DpBuilder.WorkerManager; }

        public ILogger Logger { get; private set; }

        public IIoCContainer container { get; private set; }

        protected override void BeforeInit()
        {
            container = new DryIocContainer();

            container.RegisterInstance(container);

            container.RegisterInstance<ICliProgram>(cli);
            container.RegisterInstance<ICliUser>(cli);

            container.RegisterSingleton(typeof(ILogger), typeof(ConsoleLogger));            

            container.Register<IOpcUaConnection, OpcUaConnection>();
            container.Register<IMakeHeartBeat, MakeHeartBeat>();


            container.RegisterSingleton(typeof(IDpConnectionManager), typeof(ContainerizedConnectionManager));
            container.RegisterSingleton(typeof(IDpWorkerManager), typeof(ContainerizedWorkerManager));
            container.Register<IDpBuilder, DpXmlBuilder>();

            container.Register<IMainViewModel, MainViewModel>();

            DpBuilder = container.Resolve<IDpBuilder>();            

            Logger = container.Resolve<ILogger>();            
        }

        protected override void Init()
        {
            DpBuilder.Build();
            
            mainWindow = new MainView();
            mainWindow.DataContext = container.Resolve<IMainViewModel>();

            DpConnectionManager.OpenConnections();            
        }

        protected override void AfterInit() 
        {
            
        }
    }
}
