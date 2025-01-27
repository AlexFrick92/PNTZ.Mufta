using Desktop.Application;


using DpConnect;
using DpConnect.Building;
using DpConnect.Configuration.Xml;
using DpConnect.Connection;
using DpConnect.OpcUa;

using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.Logging;
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

        const string appSettingsKey_cliLogLayout = "cliLogLayout";

        protected override void BeforeInit()
        {
            AppInstance = this;
            CurrentDirectory = currentDirectory;


            //В контейнере регистрируем все необходимые классы для работы с ПЛК
            container = new DryIocContainer();

            container.RegisterInstance(container);

            container.RegisterInstance<ICliProgram>(cli);
            container.RegisterInstance<ICliUser>(cli);
            
            Logger = NLogManager.GetLogger("logger");


            CliTarget cliTarget = new CliTarget("cli", cli)
            {
                Layout = ConfigurationManager.AppSettings[appSettingsKey_cliLogLayout]
            };            
            NLog.LogManager.Configuration.AddTarget(cliTarget);
            NLog.LogManager.Configuration.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, cliTarget);
            NLog.LogManager.ReconfigExistingLoggers();

            //container.RegisterInstance<ICliUser>(cliTarget);

            Logger.Info("********** ЗАПУСК *************");


            container.RegisterInstance<ILogger>(Logger);
            
            container.RegisterSingleton(typeof(RepositoryContext), typeof(RepositoryContext));
            
            container.Register<IDpConfigurableConnection<OpcUaConnectionConfiguration>, OpcUaConnection>();
            container.Register<MakeHeartBeat, MakeHeartBeat>();
            container.Register<RecipeToPlc, RecipeToPlc>();
            container.Register<MachineParamFromPlc, MachineParamFromPlc>();
            container.Register<HeartbeatCheck, HeartbeatCheck>();
            container.Register<JointResultDpWorker, JointResultDpWorker>();

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
