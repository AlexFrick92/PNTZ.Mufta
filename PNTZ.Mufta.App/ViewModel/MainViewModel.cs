using PNTZ.Mufta.Launcher.View.Chart;
using PNTZ.Mufta.Launcher.ViewModel.Chart;
using Toolkit.IO;
using Promatis.Desktop.Control;
using Promatis.Desktop.MVVM;
using System.ComponentModel;
using System.Windows;

namespace PNTZ.Mufta.Launcher.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        Cli _cli;
        public MainViewModel(Cli cli, ChartViewModel chartViewModel)
        {
            CliViewModel = new CliViewModel(cli);
            _chartViewModel = chartViewModel;
            _cli = cli;

            _cli.RegisterCommand("showchart", (arg) => ShowChart());
        }

        public CliViewModel CliViewModel { get; private set; }
        public UIElement MainContent { get; private set; }

        ChartViewModel _chartViewModel;

        void ShowChart()
        {
            MainContent = new TnTqChart(_chartViewModel);
            OnPropertyChanged(nameof(MainContent));
        }

    }
}
