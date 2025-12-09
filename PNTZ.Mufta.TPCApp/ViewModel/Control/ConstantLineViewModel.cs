using Desktop.MVVM;
using System.Windows;
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
        private Brush _lineColor;
        private Brush _labelColor;
        private string _valueFormat;
        private double _fontSize;
        private FontWeight _fontWeight;
        private FontFamily _fontFamily;

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
        /// Цвет линии
        /// </summary>
        public Brush LineColor
        {
            get => _lineColor;
            set => _lineColor = value;
        }

        /// <summary>
        /// Цвет текста лейбла
        /// </summary>
        public Brush LabelColor
        {
            get => _labelColor;
            set => _labelColor = value;
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

        /// <summary>
        /// Размер шрифта для лейбла
        /// </summary>
        public double FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                OnPropertyChanged(nameof(FontSize));
            }
        }

        /// <summary>
        /// Толщина шрифта для лейбла
        /// </summary>
        public FontWeight FontWeight
        {
            get => _fontWeight;
            set
            {
                _fontWeight = value;
                OnPropertyChanged(nameof(FontWeight));
            }
        }

        /// <summary>
        /// Семейство шрифта для лейбла
        /// </summary>
        public FontFamily FontFamily
        {
            get => _fontFamily;
            set
            {
                _fontFamily = value;
                OnPropertyChanged(nameof(FontFamily));
            }
        }

        public ConstantLineViewModel()
        {
            _lineColor = Brushes.DarkRed;
            _labelColor = Brushes.DarkRed;
            _valueFormat = "F2";
            _label = string.Empty;
            _fontSize = 12.0;
            _fontWeight = FontWeights.Normal;
            _fontFamily = new FontFamily("Segoe UI");
        }

        public ConstantLineViewModel(double value, string label, Brush lineColor, Brush labelColor, string valueFormat = "F2")
        {
            _value = value;
            _label = label;
            _lineColor = lineColor;
            _labelColor = labelColor;
            _valueFormat = valueFormat;
            _fontSize = 12.0;
            _fontWeight = FontWeights.Normal;
            _fontFamily = new FontFamily("Segoe UI");
        }
    }
}
