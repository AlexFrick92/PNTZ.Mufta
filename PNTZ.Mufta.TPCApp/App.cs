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
using System.Threading.Tasks;

using Toolkit.IO;
using Toolkit.Logging;


namespace PNTZ.Mufta.TPCApp
{
    internal class App : StagedApplication
    {
        static public App AppInstance;
        public string CurrentDirectory { get; private set; }
        public IDpBuilder DpBuilder { get; private set; }
        public IDpConnectionManager DpConnectionManager { get => DpBuilder.ConnectionManager; }
        public IDpWorkerManager DpWorkerManager { get => DpBuilder.WorkerManager; }

        public ILogger Logger { get; private set; }

        public IIoCContainer container { get; private set; }

        protected override void BeforeInit()
        {
            AppInstance = this;
            CurrentDirectory = currentDirectory;

            container = new DryIocContainer();

            container.RegisterInstance(container);

            container.RegisterInstance<ICliProgram>(cli);
            container.RegisterInstance<ICliUser>(cli);

            container.RegisterSingleton(typeof(ILogger), typeof(CliLogger)); ;            

            container.Register<IOpcUaConnection, OpcUaConnection>();
            container.Register<MakeHeartBeat, MakeHeartBeat>();
            container.Register<RecipeToPlc, RecipeToPlc>();
            container.Register<MachineParamFromPlc, MachineParamFromPlc>();
            container.Register<HeartbeatCheck, HeartbeatCheck>();
            container.Register<JointOperationalParamDpWorker, JointOperationalParamDpWorker>();
            container.Register<JointResultDpWorker, JointResultDpWorker>();

            container.RegisterSingleton(typeof(IDpConnectionManager), typeof(ContainerizedConnectionManager));
            container.RegisterSingleton(typeof(IDpWorkerManager), typeof(ContainerizedWorkerManager));
            container.Register<IDpBuilder, DpXmlBuilder>();

            container.Register<MainViewModel, MainViewModel>();

            DpBuilder = container.Resolve<IDpBuilder>();            

            Logger = container.Resolve<ILogger>();            
        }

        protected override void Init()
        {
            DpBuilder.Build();
            
            mainWindow = new MainView();
            mainWindow.DataContext = container.Resolve<MainViewModel>();

            cli.RegisterCommand("start", (args) => Task.Run(()=> DpConnectionManager.OpenConnections()));
            cli.RegisterCommand("stop", (args) => Task.Run(() => DpConnectionManager.CloseConnections()));
        }

        protected override void AfterInit() 
        {
            
        }
    }
}
