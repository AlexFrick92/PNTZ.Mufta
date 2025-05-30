using System;
using System.Collections;
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

namespace PNTZ.Mufta.TPCApp.View.Control
{
    /// <summary>
    /// Interaction logic for ChartSeriesView.xaml
    /// </summary>
    public partial class ChartSeriesView : UserControl
    {
        public ChartSeriesView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(ChartSeriesView));

        public static readonly DependencyProperty ChartConfigProperty =
            DependencyProperty.Register(nameof(ChartConfig), typeof(ChartViewConfig), typeof(ChartSeriesView));

        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(nameof(Series), typeof(IEnumerable), typeof(ChartSeriesView));

        public static readonly DependencyProperty ArgumentProperty =
            DependencyProperty.Register(nameof(Argument), typeof(string), typeof(ChartSeriesView));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(ChartSeriesView));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public ChartViewConfig ChartConfig
        {
            get => (ChartViewConfig)GetValue(ChartConfigProperty);
            set => SetValue(ChartConfigProperty, value);
        }

        public IEnumerable Series
        {
            get => (IEnumerable)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        public string Argument
        {
            get => (string)GetValue(ArgumentProperty);
            set => SetValue(ArgumentProperty, value);
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
