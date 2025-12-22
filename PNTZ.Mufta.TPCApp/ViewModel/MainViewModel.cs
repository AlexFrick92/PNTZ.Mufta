
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
using PNTZ.Mufta.TPCApp.ViewModel.Joint;
using PNTZ.Mufta.TPCApp.ViewModel.Recipe;
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

        RecipesView RecipeView { get; set; }
        JointView jointView { get; set; }
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
        public MainViewModel(IDpWorkerManager workerManager, 
            IDpConnectionManager connectionManager, 
            ICliProgram cli, 
            ICliUser cliUI, 
            ILogger logger, 
            LocalRepository repositoryContext)
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

            RecipeView = new RecipesView();
            RecipesViewModel recipesViewModel = new RecipesViewModel(repositoryContext, new ActualRecipe(workerManager.ResolveWorker<RecipeDpWorker>().First()));            
            RecipeView.DataContext = recipesViewModel;


            //MV View для параметров машины
            MachineParamView = new MachineParamView();
            MachineParamView.DataContext = new MachinParamViewModel(workerManager.ResolveWorker<MachineParamFromPlc>().First(), cli);

            jointView = new JointView();
            //jointView.DataContext=  new JointViewModel(workerManager.ResolveWorker<JointProcessDpWorker>().First(),
            //            recViewModel as IRecipeLoader,
            //            logger                      
            //            );

            ResultsView = new JointResultsView();
            ResultsView.DataContext = new ResultsViewModel(repositoryContext, logger);

            RecipeViewModel recViewModel = new RecipeViewModel(workerManager.ResolveWorker<RecipeDpWorker>().First(), logger, repositoryContext);
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
                (RecipeView.DataContext as RecipeViewModel)?.RefreshRecipes();
            });
            NaviToJointViewCommand = new RelayCommand((p) => MainContent = jointView);            
            NaviToMpViewCommand = new RelayCommand((p) => MainContent = MachineParamView);
            NaviToResultViewCommand = new RelayCommand((p) =>
            {
                MainContent = ResultsView;
                (ResultsView.DataContext as ResultsViewModel)?.RefreshResults();
            });

            //Кнопки подключения к ПЛК
            cli.RegisterCommand("start", (args) => connectionManager.OpenConnections());
            cli.RegisterCommand("stop", (args) => connectionManager.CloseConnections());
            cli.RegisterCommand("rr_init", (args) => repositoryContext.InitRepository());
            cli.RegisterCommand("rr_syncrecipes", (args) => repositoryContext.SyncRecipes());
            cli.RegisterCommand("rr_pushresults", (args) => repositoryContext.PushResults());
            cli.RegisterCommand("rr_pullresults", (args) => repositoryContext.PullResults(string.Join(" ", args)));
            cli.RegisterCommand("rr_clearresults", (args) => repositoryContext.ClearLocalResults());
            cli.RegisterCommand("rr_fetchresults", (args) => repositoryContext.FetchRemoteResultsNames());
            cli.RegisterCommand("shoulder", (args) =>
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    (jointView.DataContext as JointProcessViewModel).ResearchShoulder() ))
            );

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
