using PNTZ.Mufta.App.ViewModel;
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

namespace PNTZ.Mufta.App.View.MachineParameters
{
    /// <summary>
    /// Interaction logic for MachineParametersView.xaml
    /// </summary>
    public partial class MachineParametersView : UserControl
    {
        public MachineParametersView(MachineParametersViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }
    }
}
