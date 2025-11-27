using DevExpress.Xpf.Charts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for RealtimeChartView.xaml
    /// </summary>
    public partial class RealtimeChartView : UserControl
    {
        public RealtimeChartView()
        {
            InitializeComponent();
            XConstantLines = new ObservableCollection<ConstantLineModel>();
            YConstantLines = new ObservableCollection<ConstantLineModel>();
        }

        // Заголовок графика
        public static readonly DependencyProperty ChartTitleProperty =
            DependencyProperty.Register(nameof(ChartTitle), typeof(string), typeof(RealtimeChartView),
                new PropertyMetadata("График"));

        public string ChartTitle
        {
            get => (string)GetValue(ChartTitleProperty);
            set => SetValue(ChartTitleProperty, value);
        }

        // Данные для графика
        public static readonly DependencyProperty ChartDataProperty =
            DependencyProperty.Register(nameof(ChartData), typeof(IEnumerable), typeof(RealtimeChartView),
                new PropertyMetadata(null));

        public IEnumerable ChartData
        {
            get => (IEnumerable)GetValue(ChartDataProperty);
            set => SetValue(ChartDataProperty, value);
        }

        // Имя свойства для оси X
        public static readonly DependencyProperty ArgumentMemberProperty =
            DependencyProperty.Register(nameof(ArgumentMember), typeof(string), typeof(RealtimeChartView),
                new PropertyMetadata("Turns"));

        public string ArgumentMember
        {
            get => (string)GetValue(ArgumentMemberProperty);
            set => SetValue(ArgumentMemberProperty, value);
        }

        // Имя свойства для оси Y
        public static readonly DependencyProperty ValueMemberProperty =
            DependencyProperty.Register(nameof(ValueMember), typeof(string), typeof(RealtimeChartView),
                new PropertyMetadata("Torque"));

        public string ValueMember
        {
            get => (string)GetValue(ValueMemberProperty);
            set => SetValue(ValueMemberProperty, value);
        }

        // Диапазоны осей
        public static readonly DependencyProperty XMinProperty =
            DependencyProperty.Register(nameof(XMin), typeof(double), typeof(RealtimeChartView),
                new PropertyMetadata(0.0));

        public double XMin
        {
            get => (double)GetValue(XMinProperty);
            set => SetValue(XMinProperty, value);
        }

        public static readonly DependencyProperty XMaxProperty =
            DependencyProperty.Register(nameof(XMax), typeof(double), typeof(RealtimeChartView),
                new PropertyMetadata(100.0));

        public double XMax
        {
            get => (double)GetValue(XMaxProperty);
            set => SetValue(XMaxProperty, value);
        }

        public static readonly DependencyProperty YMinProperty =
            DependencyProperty.Register(nameof(YMin), typeof(double), typeof(RealtimeChartView),
                new PropertyMetadata(0.0));

        public double YMin
        {
            get => (double)GetValue(YMinProperty);
            set => SetValue(YMinProperty, value);
        }

        public static readonly DependencyProperty YMaxProperty =
            DependencyProperty.Register(nameof(YMax), typeof(double), typeof(RealtimeChartView),
                new PropertyMetadata(100.0));

        public double YMax
        {
            get => (double)GetValue(YMaxProperty);
            set => SetValue(YMaxProperty, value);
        }

        // Шаг сетки
        public static readonly DependencyProperty XGridSpacingProperty =
            DependencyProperty.Register(nameof(XGridSpacing), typeof(double), typeof(RealtimeChartView),
                new PropertyMetadata(10.0));

        public double XGridSpacing
        {
            get => (double)GetValue(XGridSpacingProperty);
            set => SetValue(XGridSpacingProperty, value);
        }

        public static readonly DependencyProperty YGridSpacingProperty =
            DependencyProperty.Register(nameof(YGridSpacing), typeof(double), typeof(RealtimeChartView),
                new PropertyMetadata(10.0));

        public double YGridSpacing
        {
            get => (double)GetValue(YGridSpacingProperty);
            set => SetValue(YGridSpacingProperty, value);
        }

        // Стиль линии
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register(nameof(LineColor), typeof(SolidColorBrush), typeof(RealtimeChartView),
                new PropertyMetadata(Brushes.Red));

        public SolidColorBrush LineColor
        {
            get => (SolidColorBrush)GetValue(LineColorProperty);
            set => SetValue(LineColorProperty, value);
        }

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register(nameof(LineThickness), typeof(double), typeof(RealtimeChartView),
                new PropertyMetadata(2.0));

        public double LineThickness
        {
            get => (double)GetValue(LineThicknessProperty);
            set => SetValue(LineThicknessProperty, value);
        }

        // Константные линии для оси X
        public static readonly DependencyProperty XConstantLinesProperty =
            DependencyProperty.Register(nameof(XConstantLines), typeof(ObservableCollection<ConstantLineModel>),
                typeof(RealtimeChartView), new PropertyMetadata(null));

        public ObservableCollection<ConstantLineModel> XConstantLines
        {
            get => (ObservableCollection<ConstantLineModel>)GetValue(XConstantLinesProperty);
            set => SetValue(XConstantLinesProperty, value);
        }

        // Константные линии для оси Y
        public static readonly DependencyProperty YConstantLinesProperty =
            DependencyProperty.Register(nameof(YConstantLines), typeof(ObservableCollection<ConstantLineModel>),
                typeof(RealtimeChartView), new PropertyMetadata(null));

        public ObservableCollection<ConstantLineModel> YConstantLines
        {
            get => (ObservableCollection<ConstantLineModel>)GetValue(YConstantLinesProperty);
            set => SetValue(YConstantLinesProperty, value);
        }

        public static readonly DependencyProperty ResetZoomTriggerProperty =
            DependencyProperty.Register(
            nameof(ResetZoomTrigger),
        typeof(object),
        typeof(RealtimeChartView),
        new PropertyMetadata(null, OnResetZoomTriggerChanged));

        public object ResetZoomTrigger
        {
            get => (object)GetValue(ResetZoomTriggerProperty);
            set => SetValue(ResetZoomTriggerProperty, value);
        }

        private static void OnResetZoomTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RealtimeChartView)d;
            control.ResetZoom();                            
        }

        private void ResetZoom()
        {            
            diagram.ActualAxisX.ActualVisualRange.SetAuto();
            diagram.ActualAxisY.ActualVisualRange.SetAuto();
        }


    }
}
