using Desktop.MVVM;
using System.Windows.Media;

namespace PNTZ.Mufta.TPCApp.ViewModel.Control
{
    /// <summary>
    /// ViewModel для описания одной серии данных на графике.
    /// Каждая серия имеет своё свойство ValueMember, цвет и толщину линии.
    /// </summary>
    /// <example>
    /// Использование в коде:
    /// <code>
    /// var series1 = new ChartSeriesViewModel
    /// {
    ///     ValueMember = "Torque",
    ///     DisplayName = "Крутящий момент",
    ///     LineColor = Brushes.Blue,
    ///     LineThickness = 2.5
    /// };
    ///
    /// var series2 = new ChartSeriesViewModel
    /// {
    ///     ValueMember = "Pressure",
    ///     DisplayName = "Давление",
    ///     LineColor = Brushes.Red,
    ///     LineThickness = 2.0
    /// };
    ///
    /// chartViewModel.Series.Add(series1);
    /// chartViewModel.Series.Add(series2);
    /// </code>
    /// </example>
    public class ChartSeriesViewModel : BaseViewModel
    {
        private string _valueMember;
        private string _displayName;
        private SolidColorBrush _lineColor;
        private double _lineThickness;

        /// <summary>
        /// Имя свойства для оси Y (значение)
        /// </summary>
        public string ValueMember
        {
            get => _valueMember;
            set
            {
                _valueMember = value;
                OnPropertyChanged(nameof(ValueMember));
            }
        }

        /// <summary>
        /// Отображаемое имя серии (для легенды)
        /// </summary>
        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        /// <summary>
        /// Цвет линии серии
        /// </summary>
        public SolidColorBrush LineColor
        {
            get => _lineColor;
            set
            {
                _lineColor = value;
                OnPropertyChanged(nameof(LineColor));
            }
        }

        /// <summary>
        /// Толщина линии серии
        /// </summary>
        public double LineThickness
        {
            get => _lineThickness;
            set
            {
                _lineThickness = value;
                OnPropertyChanged(nameof(LineThickness));
            }
        }

        public ChartSeriesViewModel()
        {
            // Значения по умолчанию
            _valueMember = string.Empty;
            _displayName = string.Empty;
            _lineColor = Brushes.Blue;
            _lineThickness = 2.0;
        }

        public ChartSeriesViewModel(string valueMember, string displayName, SolidColorBrush lineColor, double lineThickness = 2.0)
        {
            _valueMember = valueMember;
            _displayName = displayName;
            _lineColor = lineColor;
            _lineThickness = lineThickness;
        }
    }
}
