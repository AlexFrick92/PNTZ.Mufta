using Desktop.MVVM;
using System.Windows.Media;

namespace PNTZ.Mufta.TPCApp.ViewModel.Control
{
    /// <summary>
    /// ViewModel для отображения выделенной области (Strip) на графике
    /// </summary>
    public class StripViewModel : BaseViewModel
    {
        private double _minValue;
        private double _maxValue;
        private Brush _color;

        /// <summary>
        /// Минимальное значение диапазона
        /// </summary>
        public double MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                OnPropertyChanged(nameof(MinValue));
            }
        }

        /// <summary>
        /// Максимальное значение диапазона
        /// </summary>
        public double MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                OnPropertyChanged(nameof(MaxValue));
            }
        }

        /// <summary>
        /// Цвет выделенной области
        /// </summary>
        public Brush Color
        {
            get => _color;
            set => _color = value;
        }

        public StripViewModel()
        {
            _color = new SolidColorBrush(Colors.LightBlue) { Opacity = 0.3 };
            _minValue = 0;
            _maxValue = 0;
        }

        public StripViewModel(double minValue, double maxValue, Brush color)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _color = color;
        }
    }
}
