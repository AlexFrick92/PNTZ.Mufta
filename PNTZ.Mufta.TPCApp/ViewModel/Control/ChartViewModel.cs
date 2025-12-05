using Desktop.MVVM;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace PNTZ.Mufta.TPCApp.ViewModel.Control
{
    /// <summary>
    /// ViewModel для ChartView.
    /// Управляет данными графика, включая коллекции константных линий для осей X и Y.
    /// </summary>
    /// <example>
    /// Использование в XAML:
    /// <code>
    /// &lt;control:ChartView DataContext="{Binding MyChartViewModel}" /&gt;
    /// </code>
    ///
    /// Использование в C#:
    /// <code>
    /// public class MyViewModel : BaseViewModel
    /// {
    ///     public ChartViewModel MyChartViewModel { get; set; }
    ///
    ///     public MyViewModel()
    ///     {
    ///         MyChartViewModel = new ChartViewModel
    ///         {
    ///             ChartTitle = "График крутящего момента",
    ///             ArgumentMember = "Turns",
    ///             ValueMember = "Torque",
    ///             XMin = 0, XMax = 100,
    ///             YMin = 0, YMax = 10000,
    ///             LineColor = Brushes.Blue,
    ///             LineThickness = 2.5
    ///         };
    ///
    ///         // Добавление константных линий для оси X (например, обороты)
    ///         MyChartViewModel.XConstantLines.Add(new ConstantLineViewModel
    ///         {
    ///             Value = 10.5,
    ///             Label = "Мин",
    ///             Color = Brushes.DarkRed,
    ///             ValueFormat = "F2"  // 2 знака после запятой: "10.50"
    ///         });
    ///
    ///         // Или с использованием конструктора
    ///         MyChartViewModel.XConstantLines.Add(new ConstantLineViewModel(10.5, "Мин", Brushes.DarkRed, "F2"));
    ///
    ///         // Добавление константных линий для оси Y (например, момент)
    ///         MyChartViewModel.YConstantLines.Add(new ConstantLineViewModel
    ///         {
    ///             Value = 5000,
    ///             Label = "Макс",
    ///             Color = Brushes.OrangeRed,
    ///             ValueFormat = "F0"  // Целое число: "5000"
    ///         });
    ///
    ///         // Сброс зума графика (при необходимости)
    ///         // MyChartViewModel.ResetZoomTrigger = new object();
    ///     }
    /// }
    ///
    /// // ValueFormat поддерживает стандартные форматы .NET:
    /// // "F0" - целое число
    /// // "F1" - 1 знак после запятой
    /// // "F2" - 2 знака после запятой
    /// // "N0" - целое число с разделителями тысяч
    /// // "N2" - 2 знака после запятой с разделителями тысяч
    /// </code>
    /// </example>
    public class ChartViewModel : BaseViewModel
    {
        private string _chartTitle;
        private IEnumerable _chartData;
        private string _argumentMember;
        private string _valueMember;
        private double _xMin;
        private double _xMax;
        private double _yMin;
        private double _yMax;
        private double _xGridSpacing;
        private double _yGridSpacing;
        private SolidColorBrush _lineColor;
        private double _lineThickness;
        private object _resetZoomTrigger;
        private ObservableCollection<ConstantLineViewModel> _xConstantLines;
        private ObservableCollection<ConstantLineViewModel> _yConstantLines;

        /// <summary>
        /// Заголовок графика
        /// </summary>
        public string ChartTitle
        {
            get => _chartTitle;
            set => _chartTitle = value;
        }

        /// <summary>
        /// Данные для графика
        /// </summary>
        public IEnumerable ChartData
        {
            get => _chartData;
            set
            {
                _chartData = value;
                OnPropertyChanged(nameof(ChartData));
            }
        }

        /// <summary>
        /// Имя свойства для оси X (аргумент)
        /// </summary>
        public string ArgumentMember
        {
            get => _argumentMember;
            set => _argumentMember = value;  
        }

        /// <summary>
        /// Имя свойства для оси Y (значение)
        /// </summary>
        public string ValueMember
        {
            get => _valueMember;
            set => _valueMember = value;
        }

        /// <summary>
        /// Минимальное значение оси X
        /// </summary>
        public double XMin
        {
            get => _xMin;
            set => _xMin = value;
        }

        /// <summary>
        /// Максимальное значение оси X
        /// </summary>
        public double XMax
        {
            get => _xMax;
            set => _xMax = value;
        }

        /// <summary>
        /// Минимальное значение оси Y
        /// </summary>
        public double YMin
        {
            get => _yMin;
            set => _yMin = value;
        }

        /// <summary>
        /// Максимальное значение оси Y
        /// </summary>
        public double YMax
        {
            get => _yMax;
            set 
            {
                _yMax = value;
                OnPropertyChanged(nameof(YMax));
            }
        }

        /// <summary>
        /// Шаг сетки для оси X
        /// </summary>
        public double XGridSpacing
        {
            get => _xGridSpacing;
            set => _xGridSpacing = value;
        }

        /// <summary>
        /// Шаг сетки для оси Y
        /// </summary>
        public double YGridSpacing
        {
            get => _yGridSpacing;
            set => _yGridSpacing = value;
        }

        /// <summary>
        /// Цвет линии графика
        /// </summary>
        public SolidColorBrush LineColor
        {
            get => _lineColor;
            set => _lineColor = value;
        }

        /// <summary>
        /// Толщина линии графика
        /// </summary>
        public double LineThickness
        {
            get => _lineThickness;
            set => _lineThickness = value;
        }

        /// <summary>
        /// Триггер для сброса зума графика. Измените это свойство, чтобы сбросить зум.
        /// </summary>
        public object ResetZoomTrigger
        {
            get => _resetZoomTrigger;
            set => _resetZoomTrigger = value;
        }

        /// <summary>
        /// Коллекция константных линий для оси X
        /// </summary>
        public ObservableCollection<ConstantLineViewModel> XConstantLines
        {
            get => _xConstantLines;
            set => _xConstantLines = value;
        }

        /// <summary>
        /// Коллекция константных линий для оси Y
        /// </summary>
        public ObservableCollection<ConstantLineViewModel> YConstantLines
        {
            get => _yConstantLines;
            set => _yConstantLines = value;
        }

        public ChartViewModel()
        {
            // Значения по умолчанию
            _chartTitle = "График";
            _argumentMember = "Turns";
            _valueMember = "Torque";
            _xMin = 0.0;
            _xMax = 100.0;
            _yMin = 0.0;
            _yMax = 100.0;
            _xGridSpacing = 10.0;
            _yGridSpacing = 10.0;
            _lineColor = Brushes.Red;
            _lineThickness = 2.0;

            _xConstantLines = new ObservableCollection<ConstantLineViewModel>();
            _yConstantLines = new ObservableCollection<ConstantLineViewModel>();
        }
    }
}
