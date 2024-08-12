using DevExpress.Xpf.Charts;
using PNTZ.Mufta.Launcher.ViewModel.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PNTZ.Mufta.Launcher.View.Chart
{
    /// <summary>
    /// Interaction logic for TnTqChart.xaml
    /// </summary>
    public partial class TnTqChart : System.Windows.Controls.UserControl
    {
        public TnTqChart(ChartViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;            
        }
    }
}
