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
using Desktop.Control;
using Desktop.MVVM;

namespace Desktop.Layout
{
    /// <summary>
    /// Interaction logic for RootControl.xaml
    /// </summary>
    public partial class RootControl : UserControl, INotifyPropertyChanged
    {
        public RootControl()
        {
            Loaded += RootControl_Loaded;

            InitializeComponent();
        }
        private void RootControl_Loaded(object sender, RoutedEventArgs e)
        {            
            OutputBarTabbed tabbedBar = new OutputBarTabbed(OutputElementCollection);              
            OutputBar = tabbedBar;
            tabbedBar.DataContext = DataContext;
            OnPropertyChanged(nameof(OutputBar));
        }



        public ObservableCollection<FrameworkElement> OutputElementCollection { get; set; } = new ObservableCollection<FrameworkElement>();

        public static DependencyProperty StatusBarProperty =
            DependencyProperty.Register(nameof(StatusBar), typeof(UIElement), typeof(RootControl)                
                );

        public UIElement StatusBar
        {
            get { return (UIElement)GetValue(StatusBarProperty); }
            set { SetValue(StatusBarProperty, value); }
        }

        public UIElement OutputBar { get; private set; }

        public static DependencyProperty ToolBarProperty =
            DependencyProperty.Register(nameof(ToolBar), typeof(UIElement), typeof(RootControl));

        public UIElement ToolBar
        {
            get { return (UIElement)GetValue(ToolBarProperty); }
            set { SetValue(ToolBarProperty, value); }
        }

        public static DependencyProperty MainContentProperty =
            DependencyProperty.Register(nameof(MainContent), typeof(UIElement), typeof(RootControl));

        public UIElement MainContent
        {
            get { return (UIElement)GetValue(MainContentProperty); }
            set { SetValue(MainContentProperty, value); }
        }

        public static DependencyProperty LeftSidebarProperty =
            DependencyProperty.Register(nameof(LeftSidebar), typeof(UIElement), typeof(RootControl));

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public UIElement LeftSidebar
        {
            get { return (UIElement)GetValue(LeftSidebarProperty); }
            set { SetValue(LeftSidebarProperty, value); }
        }


    }
}
