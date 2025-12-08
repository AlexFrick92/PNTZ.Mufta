using Desktop.MVVM;
using DevExpress.Xpf.Charts;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace PNTZ.Mufta.TPCApp.ViewModel.Control
{
    /// <summary>
    /// ViewModel для ChartView
    /// </summary>
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
        private string _xAxisTitle;
        private string _yAxisTitle;
        private IAxisLabelFormatter _xAxisLabelFormatter;
        private IAxisLabelFormatter _yAxisLabelFormatter;

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

        /// <summary>
        /// Заголовок оси X
        /// </summary>
        public string XAxisTitle
        {
            get => _xAxisTitle;
            set
            {
                _xAxisTitle = value;
                OnPropertyChanged(nameof(XAxisTitle));
            }
        }

        /// <summary>
        /// Заголовок оси Y
        /// </summary>
        public string YAxisTitle
        {
            get => _yAxisTitle;
            set
            {
                _yAxisTitle = value;
                OnPropertyChanged(nameof(YAxisTitle));
            }
        }

        /// <summary>
        /// Форматтер меток оси X
        /// </summary>
        public IAxisLabelFormatter XAxisLabelFormatter
        {
            get => _xAxisLabelFormatter;
            set
            {
                _xAxisLabelFormatter = value;
                OnPropertyChanged(nameof(XAxisLabelFormatter));
            }
        }

        /// <summary>
        /// Форматтер меток оси Y
        /// </summary>
        public IAxisLabelFormatter YAxisLabelFormatter
        {
            get => _yAxisLabelFormatter;
            set
            {
                _yAxisLabelFormatter = value;
                OnPropertyChanged(nameof(YAxisLabelFormatter));
            }
        }

        public ChartViewModel()
        {
            // Значения по умолчанию            
            _argumentMember = "Аргумент";
            _valueMember = "Значение";
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
