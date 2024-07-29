using PNTZ.Mufta.Launcher.View.Chart;
using Promatis.DebuggingToolkit.IO;
using Promatis.Desktop.Control;
using Promatis.Desktop.MVVM;
using System.ComponentModel;
using System.Windows;

namespace PNTZ.Mufta.Launcher.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        Cli _cli;
        public MainViewModel(Cli cli)
        {
            CliViewModel = new CliViewModel(cli);
            _cli = cli;

            _cli.RegisterCommand("showchart", (arg) => ShowChart());
        }

        public CliViewModel CliViewModel { get; private set; }
        public UIElement MainContent { get; private set; }

        void ShowChart()
        {
            MainContent = new TnTqChart(new Chart.ChartViewModel());
            OnPropertyChanged(nameof(MainContent));
        }

    }
}
