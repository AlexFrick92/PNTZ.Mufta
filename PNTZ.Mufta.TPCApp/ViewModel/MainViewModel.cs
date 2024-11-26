
using Desktop.Control;
using Desktop.MVVM;

using DpConnect;
using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.View.Joint;
using PNTZ.Mufta.TPCApp.View.Recipe;
using Promatis.Core.Logging;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Toolkit.IO;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        IDpWorkerManager WorkerManager { get; set; }
        IDpConnectionManager ConnectionManager { get; set; }

        public CliViewModel CliViewModel { get; set; }


        CreateRecipeView createRecipeView { get; set; }
        JointView jointView { get; set; }
        public ICommand NaviToRecipeViewCommand { get; private set; }
        public ICommand NaviToJointViewCommand { get; private set; }
        public ICommand NaviToMpViewCommand { get; private set; }        


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
        public MainViewModel(IDpWorkerManager workerManager, IDpConnectionManager connectionManager, ICliProgram cli, ICliUser cliUI, ILogger logger)
        {
            this.WorkerManager = workerManager;
            this.ConnectionManager = connectionManager;

            CliViewModel = new CliViewModel(cliUI);


            MachineParamFromPlc mpListener = workerManager.ResolveWorker<MachineParamFromPlc>().First();
            cli.RegisterCommand("startmp", async (arg) => await mpListener.StartAwaitingForMpAsync());
            cli.RegisterCommand("stopmp", (arg) => mpListener.StopAwaitingForMp());

            createRecipeView = new CreateRecipeView();
            createRecipeView.DataContext = new RecipeViewModel(workerManager.ResolveWorker<RecipeToPlc>().First(), logger);

            jointView = new JointView();

            NaviToRecipeViewCommand = new RelayCommand((p) => MainContent = createRecipeView);
            NaviToJointViewCommand = new RelayCommand((p) => MainContent = jointView);
            //NaviToMpViewCommand = new RelayCommand((p) => MainContent = MachineParametersView);
        }
    }
}
