using Desktop.Application;


using DpConnect;
using DpConnect.Building;
using DpConnect.Configuration.Xml;
using DpConnect.Connection;
using DpConnect.OpcUa;

using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.View;
using PNTZ.Mufta.TPCApp.ViewModel;

using Promatis.Core;
using Promatis.Core.Logging;
using Promatis.Core.Results;
using Promatis.IoC.DryIoc;
using Promatis.Logging.NLog;

using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

using Toolkit.IO;
using Toolkit.Logging;



namespace PNTZ.Mufta.TPCApp
{
    /// <summary>
    /// Класс унаследованный от StagedAplication
    /// StagedApplication - база для wpf приложения, в которой организована
    /// поэтапный запуск приложения. 
    /// Сначала показывается окно загрузки
    /// В это время можно запустить процесс инициализации
    /// Затем открывается основное окно    
    /// </summary>
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
            
            Logger = NLogManager.GetLogger("logger");
            CliLogger cliLogger = new CliLogger(cli, Logger);

            //В контейнере регистрируем все необходимые классы для работы с ПЛК
            container = new DryIocContainer();

            container.RegisterInstance(container);

            container.RegisterInstance<ICliProgram>(cli);
            container.RegisterInstance<ICliUser>(cli);
            
            container.RegisterInstance<ILogger>(cliLogger);           

            Logger.Info("********** ЗАПУСК *************");            
            
            container.RegisterSingleton(typeof(RepositoryContext), typeof(RepositoryContext));
            
            container.Register<IDpConfigurableConnection<OpcUaConnectionConfiguration>, OpcUaConnection>();
            container.Register<MakeHeartBeat, MakeHeartBeat>();
            container.Register<RecipeDpWorker, RecipeDpWorker>();
            container.Register<MachineParamFromPlc, MachineParamFromPlc>();
            container.Register<HeartbeatCheck, HeartbeatCheck>();
            container.Register<JointProcessDpWorker, JointProcessDpWorker>();

            container.RegisterSingleton(typeof(IDpConnectionManager), typeof(ContainerizedConnectionManager));
            container.RegisterSingleton(typeof(IDpWorkerManager), typeof(ContainerizedWorkerManager));
            container.RegisterSingleton<IDpBinder, DpBinder>();
            container.Register<IDpBuilder, DpXmlBuilder>();

            container.Register<MainViewModel, MainViewModel>();


            Logger = container.Resolve<ILogger>();
                        
            DpBuilder = container.Resolve<IDpBuilder>();            
        }

        protected override void Init()
        {
            DpBuilder.Build();
            
            mainWindow = new MainView();
            mainWindow.DataContext = container.Resolve<MainViewModel>();
        }

        protected override void AfterInit() 
        {
            
        }
    }
}
