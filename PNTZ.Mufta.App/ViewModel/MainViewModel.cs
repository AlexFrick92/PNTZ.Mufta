using System.Windows;

using Desktop.Control;
using Desktop.MVVM;
using Toolkit.IO;

using PNTZ.Mufta.App.View.Chart;
using PNTZ.Mufta.App.ViewModel.Chart;
using PNTZ.Mufta.App.View;

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
            cli = cli;

            MainContent = new AboutView();

            cli.RegisterCommand("showchart", (arg) => ShowChart());
        }

        public CliViewModel CliViewModel { get; private set; }
        public UIElement MainContent { get; private set; }

        ChartViewModel _chartViewModel;

        void ShowChart()
        {
            MainContent = _chart;
            OnPropertyChanged(nameof(MainContent));
        }

    }
}
