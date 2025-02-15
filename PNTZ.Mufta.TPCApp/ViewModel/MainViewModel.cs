﻿
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

        CreateRecipeView createRecipeView { get; set; }
        JointView jointView { get; set; }
        MachineParamView MachineParamView { get; set; }
        ResultsView ResultsView { get; set; }
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
        public MainViewModel(IDpWorkerManager workerManager, IDpConnectionManager connectionManager, ICliProgram cli, ICliUser cliUI, ILogger logger, RepositoryContext repositoryContext)
        {
            // Загрузка конфигурации
            try
            {
                var config = XDocument.Load($"{AppInstance.CurrentDirectory}/ViewModel/MainViewModel.xml");
                connectOnStartup = bool.Parse(config.Root.Element("ConnectOnStartup").Value);

            }
            catch (Exception ex)
            {
                logger.Info("Не удалось загрузить конфигурацию для JointViewModel:");
                logger.Info(ex.Message);
                logger.Info("Будут использованы значения по-умолчанию");
            }

            // Инициализация 

            this.WorkerManager = workerManager;
            this.ConnectionManager = connectionManager;



            //Создаем ViewModels
            CliViewModel = new CliViewModel(cliUI);            

            createRecipeView = new CreateRecipeView();
            RecipeViewModel recViewModel = new RecipeViewModel(workerManager.ResolveWorker<RecipeDpWorker>().First(), logger, repositoryContext);
            createRecipeView.DataContext = recViewModel;

            MachineParamView = new MachineParamView();
            MachineParamView.DataContext = new MachinParamViewModel(workerManager.ResolveWorker<MachineParamFromPlc>().First(), cli);

            jointView = new JointView();
            jointView.DataContext=  new JointViewModel(workerManager.ResolveWorker<JointResultDpWorker>().First(),
                        recViewModel as IRecipeLoader,
                        logger,
                        cli,
                        repositoryContext
                        );

            ResultsView = new ResultsView();
            ResultsView.DataContext = new ResultsViewModel(repositoryContext);


            this.StatusBarViewModel = new StatusBarViewModel(workerManager.ResolveWorker<JointResultDpWorker>().First(), 
                workerManager.ResolveWorker<HeartbeatCheck>().First(),
                recViewModel as IRecipeLoader
                );
            OnPropertyChanged(nameof(StatusBarViewModel));



            //Навигация между окнами
            NaviToRecipeViewCommand = new RelayCommand((p) =>
            {
                MainContent = createRecipeView;                
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
