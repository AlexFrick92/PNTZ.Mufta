using System.Windows;

using Desktop.Control;
using Desktop.MVVM;
using Toolkit.IO;

using PNTZ.Mufta.App.ViewModel.Chart;
using PNTZ.Mufta.App.View;
using PNTZ.Mufta.App.View.Chart;
using PNTZ.Mufta.App.View.CreateRecipe;
using System.Windows.Input;
using PNTZ.Mufta.App.View.Joint;

namespace PNTZ.Mufta.App.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        Cli cli;
        TnTqChart _chart;
        public MainViewModel(Cli cli, ChartViewModel chartViewModel)
        {
            CliViewModel = new CliViewModel(cli);
            _chartViewModel = chartViewModel;
            _chart = new TnTqChart(chartViewModel);
            this.cli = cli;

            CreateRecipeView = new CreateRecipeView(new CreateRecipeViewModel());
            JointView = new JointView();


            NaviToRecipeViewCommand = new RelayCommand((p) => MainContent = CreateRecipeView);
            NaviToJointViewCommand = new RelayCommand((p) => MainContent = JointView);


            cli.RegisterCommand("showchart", (arg) => ShowChart());
        }

        public CreateRecipeView CreateRecipeView { get; private set; }
        public JointView JointView { get; private set; }

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

        ChartViewModel _chartViewModel;

        void ShowChart()
        {
            MainContent = _chart;
            OnPropertyChanged(nameof(MainContent));
        }

        public ICommand NaviToRecipeViewCommand {  get; private set; }
        public ICommand NaviToJointViewCommand { get; private set; }

    }
}
