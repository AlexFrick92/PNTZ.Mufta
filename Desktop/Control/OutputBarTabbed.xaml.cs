using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for OutputBarTabbed.xaml
    /// </summary>
    public partial class OutputBarTabbed : UserControl, INotifyPropertyChanged
    {
        public OutputBarTabbed(ObservableCollection<FrameworkElement> elements)
        {
            Elements = elements;
            Loaded += OutputBarTabbed_Loaded;
            InitializeComponent();
        }

        private void OutputBarTabbed_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (FrameworkElement el in Elements)
            {              
                el.Visibility = Visibility.Collapsed;
                ElementView.Children.Add(el);

                string header = (el is IOutputBarElement) ? ((IOutputBarElement)el).Header.ToUpper() : "not set";

                OutputBarButton btn = new OutputBarButton(header);
                outputBarButtons.Add(btn);
                btn.SwitchRequested += (s, v) =>
                {
                    foreach (var elem in Elements)
                        elem.Visibility = Visibility.Collapsed;

                    el.Visibility = Visibility.Visible;                 
                    foreach(var btn1 in outputBarButtons)
                        btn1.Active = false;

                    btn.Active = true;
                };

                Buttons.Children.Add(btn);
            }

            if (outputBarButtons.Count > 0)
                outputBarButtons[0].Switch.Execute(null);
            
        }
        public ObservableCollection<OutputBarButton> outputBarButtons { get; private set; } = new ObservableCollection<OutputBarButton>();
        public ObservableCollection<FrameworkElement> Elements { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
