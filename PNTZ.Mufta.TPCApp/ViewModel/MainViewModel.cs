
using Desktop.Control;
using Desktop.MVVM;

using DpConnect;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.View.Joint;
using PNTZ.Mufta.TPCApp.View.MP;
using PNTZ.Mufta.TPCApp.View.Recipe;
using PNTZ.Mufta.TPCApp.View.Results;
using Promatis.Core.Logging;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

using Toolkit.IO;

using static PNTZ.Mufta.TPCApp.App;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        IDpWorkerManager WorkerManager { get; set; }
        IDpConnectionManager ConnectionManager { get; set; }

        public CliViewModel CliViewModel { get; set; }
        public StatusBarViewModel StatusBarViewModel { get; set; }

        RecipeView RecipeView { get; set; }
        JointProcessView jointView { get; set; }
        MachineParamView MachineParamView { get; set; }
        JointResultsView ResultsView { get; set; }
        public ICommand NaviToRecipeViewCommand { get; private set; }
        public ICommand NaviToJointViewCommand { get; private set; }
        public ICommand NaviToMpViewCommand { get; private set; }
        public ICommand NaviToResultViewCommand { get; private set; }


        UIElement _mainContent = null;
        public UIElement MainContent
        {
            get { return _mainContent; }
            private set
            {
                _mainContent = value;
                OnPropertyChanged(nameof(MainContent));                
            }
        }

        bool connectOnStartup = false;
        public MainViewModel(IDpWorkerManager workerManager, IDpConnectionManager connectionManager, ICliProgram cli, ICliUser cliUI, ILogger logger, LocalRepositoryContext repositoryContext)
        {
            // Загрузка конфигурации
            try
            {
                var config = XDocument.Load($"{AppInstance.CurrentDirectory}/ViewModel/MainViewModel.xml");
                connectOnStartup = bool.Parse(config.Root.Element("ConnectOnStartup").Value);

            }
            catch (Exception ex)
            {
                logger.Info("Не удалось загрузить конфигурацию для JointProcessViewModel:");
                logger.Info(ex.Message);
                logger.Info("Будут использованы значения по-умолчанию");
            }

            // Инициализация 

            this.WorkerManager = workerManager;
            this.ConnectionManager = connectionManager;



            //Создаем ViewModels
            CliViewModel = new CliViewModel(cliUI);            

            RecipeView = new RecipeView();
            RecipeViewModel recViewModel = new RecipeViewModel(workerManager.ResolveWorker<RecipeDpWorker>().First(), logger, repositoryContext);
            RecipeView.DataContext = recViewModel;

            MachineParamView = new MachineParamView();
            MachineParamView.DataContext = new MachinParamViewModel(workerManager.ResolveWorker<MachineParamFromPlc>().First(), cli);

            jointView = new JointProcessView();
            jointView.DataContext=  new JointProcessViewModel(workerManager.ResolveWorker<JointProcessDpWorker>().First(),
                        recViewModel as IRecipeLoader,
                        logger,
                        cli,
                        repositoryContext
                        );

            ResultsView = new JointResultsView();
            ResultsView.DataContext = new ResultsViewModel(repositoryContext, logger);


            this.StatusBarViewModel = new StatusBarViewModel(workerManager.ResolveWorker<JointProcessDpWorker>().First(), 
                workerManager.ResolveWorker<HeartbeatCheck>().First(),
                recViewModel as IRecipeLoader,
                workerManager.ResolveWorker<SensorStatusDpWorker>().First()
                );
            OnPropertyChanged(nameof(StatusBarViewModel));



            //Навигация между окнами
            NaviToRecipeViewCommand = new RelayCommand((p) =>
            {
                MainContent = RecipeView;                
            });
            NaviToJointViewCommand = new RelayCommand((p) => MainContent = jointView);            
            NaviToMpViewCommand = new RelayCommand((p) => MainContent = MachineParamView);
            NaviToResultViewCommand = new RelayCommand((p) => MainContent = ResultsView);

            //Кнопки подключения к ПЛК
            cli.RegisterCommand("start", (args) => Task.Run(() => connectionManager.OpenConnections()));
            cli.RegisterCommand("stop", (args) => Task.Run(() => connectionManager.CloseConnections()));

            if (connectOnStartup)
            {
                try
                {
                    connectionManager.OpenConnections();
                }
                catch (Exception ex)
                {
                    logger.Info("При попытке подключения к ПЛК возникли ошибки:");
                    logger.Info(ex.Message);
                }                
            }
        }
    }
}
