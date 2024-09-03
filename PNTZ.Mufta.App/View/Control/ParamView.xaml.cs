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

namespace PNTZ.Mufta.App.View.Control
{
    /// <summary>
    /// Interaction logic for ParamView.xaml
    /// </summary>
    public partial class ParamView : UserControl
    {
        public ParamView()
        {
            InitializeComponent();
        }

        public string ParamLabel
        {
            get { return (string)GetValue(ParamLabelProperty); }
            set { SetValue(ParamLabelProperty, value); }
        }

        public static DependencyProperty ParamLabelProperty =
            DependencyProperty.Register(nameof(ParamLabel), typeof(string), typeof(ParamView)
        );

        public string ParamValue
        {
            get { return (string)GetValue(ParamValueProperty); }
            set { SetValue(ParamValueProperty, value); }
        }

        public static DependencyProperty ParamValueProperty =
            DependencyProperty.Register(nameof(ParamValue), typeof(string), typeof(ParamView)
        );

    }
}
