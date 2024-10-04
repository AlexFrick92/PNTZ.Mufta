using System.Windows;

using Desktop.Control;
using Desktop.MVVM;
using Toolkit.IO;

using PNTZ.Mufta.App.View;
using PNTZ.Mufta.App.View.CreateRecipe;
using System.Windows.Input;
using PNTZ.Mufta.App.View.Joint;
using PNTZ.Mufta.App.View.MachineParameters;

namespace PNTZ.Mufta.App.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        Cli cli;
        public MainViewModel(Cli cli)
        {
            CliViewModel = new CliViewModel(cli);
            this.cli = cli;

            CreateRecipeView = new CreateRecipeView(new CreateRecipeViewModel());
            JointView = new JointView(new JointViewModel());
            MachineParametersView = new MachineParametersView(new MachineParametersViewModel());


            NaviToRecipeViewCommand = new RelayCommand((p) => MainContent = CreateRecipeView);
            NaviToJointViewCommand = new RelayCommand((p) => MainContent = JointView);
            NaviToMpViewCommand = new RelayCommand((p) => MainContent = MachineParametersView);
        }

        public CreateRecipeView CreateRecipeView { get; private set; }
        public JointView JointView { get; private set; }

        public MachineParametersView MachineParametersView { get; private set; }

        public CliViewModel CliViewModel { get; private set; }

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

        public ICommand NaviToRecipeViewCommand {  get; private set; }
        public ICommand NaviToJointViewCommand { get; private set; }
        public ICommand NaviToMpViewCommand { get; private set; }

    }
}
