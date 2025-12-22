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

namespace Desktop.Control
{
    /// <summary>
    /// Interaction logic for cli.xaml
    /// </summary>
    public partial class CliView : UserControl, IOutputBarElement
    {
        public string Header { get; set; } = "terminal";

        public CliView()
        {
            InitializeComponent();
            Output.DataContextChanged += (s, v) =>
            {
                OutputScroll.ScrollToEnd();                
            };
        }

        private void Output_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Input.Focus();
        }

        private void Output_GotFocus(object sender, RoutedEventArgs e)
        {
            Input.Focus();
        }
    }
}
