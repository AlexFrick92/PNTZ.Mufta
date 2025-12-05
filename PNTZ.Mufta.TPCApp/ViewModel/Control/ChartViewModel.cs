using Desktop.MVVM;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace PNTZ.Mufta.TPCApp.ViewModel.Control
{
    /// <summary>
    /// ViewModel для ChartView.
    /// Управляет данными графика, включая коллекции серий и константных линий для осей X и Y.
    /// </summary>
    /// <example>
    /// Использование в XAML:
    /// <code>
    /// &lt;control:ChartView DataContext="{Binding MyChartViewModel}" /&gt;
    /// </code>
    ///
    /// Использование в C# (несколько серий):
    /// <code>
    /// public class MyViewModel : BaseViewModel
    /// {
    ///     public ChartViewModel MyChartViewModel { get; set; }
    ///
    ///     public MyViewModel()
    ///     {
    ///         MyChartViewModel = new ChartViewModel
    ///         {
    ///             ChartTitle = "График момента и давления",
    ///             ArgumentMember = "Turns",
    ///             XMin = 0, XMax = 100,
    ///             YMin = 0, YMax = 10000
    ///         };
    ///
    ///         // Добавление нескольких серий
    ///         MyChartViewModel.Series.Add(new ChartSeriesViewModel
    ///         {
    ///             ValueMember = "Torque",
    ///             DisplayName = "Крутящий момент",
    ///             LineColor = Brushes.Blue,
    ///             LineThickness = 2.5
    ///         });
    ///
    ///         MyChartViewModel.Series.Add(new ChartSeriesViewModel
    ///         {
    ///             ValueMember = "Pressure",
    ///             DisplayName = "Давление",
    ///             LineColor = Brushes.Red,
    ///             LineThickness = 2.0
    ///         });
    ///
    ///         // Установка данных
    ///         MyChartViewModel.ChartData = myDataCollection;
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
    ///
    /// // ПРИМЕЧАНИЕ: Старые свойства ValueMember, LineColor и LineThickness сохранены для обратной совместимости.
    /// // При установке этих свойств автоматически создаётся первая серия.
    /// </code>
    /// </example>
    public class ChartViewModel : BaseViewModel
    {
        private string _chartTitle;
        private IEnumerable _chartData;
        private string _argumentMember;
        private string _valueMember; // Deprecated: используется для обратной совместимости
        private double _xMin;
        private double _xMax;
        private double _yMin;
        private double _yMax;
        private double _xGridSpacing;
        private double _yGridSpacing;
        private SolidColorBrush _lineColor; // Deprecated: используется для обратной совместимости
        private double _lineThickness; // Deprecated: используется для обратной совместимости
        private object _resetZoomTrigger;
        private ObservableCollection<ConstantLineViewModel> _xConstantLines;
        private ObservableCollection<ConstantLineViewModel> _yConstantLines;
        private ObservableCollection<ChartSeriesViewModel> _series;

        /// <summary>
        /// Заголовок графика
        /// </summary>
        public string ChartTitle
        {
            get => _chartTitle;
            set
            {
                _chartTitle = value;
                OnPropertyChanged(nameof(ChartTitle));
            }
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
            set
            {
                _argumentMember = value;
                OnPropertyChanged(nameof(ArgumentMember));
            }
        }

        /// <summary>
        /// Имя свойства для оси Y (значение).
        /// УСТАРЕЛО: Используйте коллекцию Series для добавления серий.
        /// При установке этого свойства автоматически создаётся/обновляется первая серия.
        /// </summary>
        [Obsolete("Используйте коллекцию Series для добавления серий")]
        public string ValueMember
        {
            get => _valueMember;
            set
            {
                _valueMember = value;
                OnPropertyChanged(nameof(ValueMember));

                // Для обратной совместимости: создаём или обновляем первую серию
                if (Series.Count == 0)
                {
                    Series.Add(new ChartSeriesViewModel
                    {
                        ValueMember = value,
                        DisplayName = value,
                        LineColor = _lineColor ?? Brushes.Blue,
                        LineThickness = _lineThickness > 0 ? _lineThickness : 2.0
                    });
                }
                else
                {
                    Series[0].ValueMember = value;
                    if (string.IsNullOrEmpty(Series[0].DisplayName) || Series[0].DisplayName == _valueMember)
                    {
                        Series[0].DisplayName = value;
                    }
                }
            }
        }

        /// <summary>
        /// Минимальное значение оси X
        /// </summary>
        public double XMin
        {
            get => _xMin;
            set
            {
                _xMin = value;
                OnPropertyChanged(nameof(XMin));
            }
        }

        /// <summary>
        /// Максимальное значение оси X
        /// </summary>
        public double XMax
        {
            get => _xMax;
            set
            {
                _xMax = value;
                OnPropertyChanged(nameof(XMax));
            }
        }

        /// <summary>
        /// Минимальное значение оси Y
        /// </summary>
        public double YMin
        {
            get => _yMin;
            set
            {
                _yMin = value;
                OnPropertyChanged(nameof(YMin));
            }
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
            set
            {
                _xGridSpacing = value;
                OnPropertyChanged(nameof(XGridSpacing));
            }
        }

        /// <summary>
        /// Шаг сетки для оси Y
        /// </summary>
        public double YGridSpacing
        {
            get => _yGridSpacing;
            set
            {
                _yGridSpacing = value;
                OnPropertyChanged(nameof(YGridSpacing));
            }
        }

        /// <summary>
        /// Цвет линии графика.
        /// УСТАРЕЛО: Используйте коллекцию Series для настройки цвета серий.
        /// При установке этого свойства обновляется цвет первой серии.
        /// </summary>
        [Obsolete("Используйте коллекцию Series для настройки цвета серий")]
        public SolidColorBrush LineColor
        {
            get => _lineColor;
            set
            {
                _lineColor = value;
                OnPropertyChanged(nameof(LineColor));

                // Для обратной совместимости: обновляем цвет первой серии
                if (Series.Count > 0)
                {
                    Series[0].LineColor = value;
                }
            }
        }

        /// <summary>
        /// Толщина линии графика.
        /// УСТАРЕЛО: Используйте коллекцию Series для настройки толщины серий.
        /// При установке этого свойства обновляется толщина первой серии.
        /// </summary>
        [Obsolete("Используйте коллекцию Series для настройки толщины серий")]
        public double LineThickness
        {
            get => _lineThickness;
            set
            {
                _lineThickness = value;
                OnPropertyChanged(nameof(LineThickness));

                // Для обратной совместимости: обновляем толщину первой серии
                if (Series.Count > 0)
                {
                    Series[0].LineThickness = value;
                }
            }
        }

        /// <summary>
        /// Триггер для сброса зума графика. Измените это свойство, чтобы сбросить зум.
        /// </summary>
        public object ResetZoomTrigger
        {
            get => _resetZoomTrigger;
            set
            {
                _resetZoomTrigger = value;
                OnPropertyChanged(nameof(ResetZoomTrigger));
            }
        }

        /// <summary>
        /// Коллекция константных линий для оси X
        /// </summary>
        public ObservableCollection<ConstantLineViewModel> XConstantLines
        {
            get => _xConstantLines;
            set
            {
                _xConstantLines = value;
                OnPropertyChanged(nameof(XConstantLines));
            }
        }

        /// <summary>
        /// Коллекция константных линий для оси Y
        /// </summary>
        public ObservableCollection<ConstantLineViewModel> YConstantLines
        {
            get => _yConstantLines;
            set
            {
                _yConstantLines = value;
                OnPropertyChanged(nameof(YConstantLines));
            }
        }

        /// <summary>
        /// Коллекция серий данных для отображения на графике.
        /// Каждая серия имеет своё свойство ValueMember, цвет и толщину линии.
        /// </summary>
        public ObservableCollection<ChartSeriesViewModel> Series
        {
            get => _series;
            set
            {
                _series = value;
                OnPropertyChanged(nameof(Series));
            }
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
            _series = new ObservableCollection<ChartSeriesViewModel>();
        }
    }
}
