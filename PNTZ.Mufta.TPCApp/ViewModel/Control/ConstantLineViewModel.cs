using Desktop.MVVM;
using System.Windows.Media;

namespace PNTZ.Mufta.TPCApp.ViewModel.Control
{
    /// <summary>
    /// ViewModel для отображения константной линии на графике
    /// </summary>
    public class ConstantLineViewModel : BaseViewModel
    {
        private double _value;
        private string _label;
        private SolidColorBrush _color;
        private string _valueFormat;

        /// <summary>
        /// Значение, на котором будет отображаться линия
        /// </summary>
        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(FormattedValue));
            }
        }

        /// <summary>
        /// Текстовая метка для линии
        /// </summary>
        public string Label
        {
            get => _label;
            set => _label = value;
        }

        /// <summary>
        /// Цвет линии и текста
        /// </summary>
        public SolidColorBrush Color
        {
            get => _color;
            set => _color = value;
        }

        /// <summary>
        /// Формат отображения значения (например: "F0" для целых чисел, "F2" для 2 знаков после запятой)
        /// </summary>
        public string ValueFormat
        {
            get => _valueFormat;
            set
            {
                _valueFormat = value;
                OnPropertyChanged(nameof(ValueFormat));
                OnPropertyChanged(nameof(FormattedValue));
            }
        }

        /// <summary>
        /// Отформатированное значение согласно ValueFormat
        /// </summary>
        public string FormattedValue
        {
            get
            {
                if (string.IsNullOrEmpty(_valueFormat))
                    return _value.ToString();

                return _value.ToString(_valueFormat);
            }
        }

        public ConstantLineViewModel()
        {
            _color = Brushes.DarkRed;
            _valueFormat = "F2";
            _label = string.Empty;
        }

        public ConstantLineViewModel(double value, string label, SolidColorBrush color, string valueFormat = "F2")
        {
            _value = value;
            _label = label;
            _color = color;
            _valueFormat = valueFormat;
        }
    }
}
