using System;
using System.Windows;
using System.Windows.Controls;

namespace PNTZ.Mufta.TPCApp.View.Control
{
    /// <summary>
    /// Состояние параметра для визуального отображения
    /// </summary>
    public enum ParameterState
    {
        Normal,  // обычный цвет (черный)
        Good,    // зелёный
        Bad      // красный
    }

    /// <summary>
    /// Контрол для отображения параметра: Label + значение с цветовой индикацией состояния
    /// </summary>
    public partial class ParameterDisplayControl : UserControl
    {
        public ParameterDisplayControl()
        {
            InitializeComponent();
        }

        // Label - название параметра
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                nameof(Label),
                typeof(string),
                typeof(ParameterDisplayControl),
                new PropertyMetadata(string.Empty));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Value - значение параметра (object для универсальности)
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(object),
                typeof(ParameterDisplayControl),
                new PropertyMetadata(null, OnValueOrFormatChanged));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // StringFormat - формат отображения (N0, F1, F2 и т.д.)
        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register(
                nameof(StringFormat),
                typeof(string),
                typeof(ParameterDisplayControl),
                new PropertyMetadata(string.Empty, OnValueOrFormatChanged));

        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        // State - состояние параметра для изменения цвета
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(
                nameof(State),
                typeof(ParameterState),
                typeof(ParameterDisplayControl),
                new PropertyMetadata(ParameterState.Normal));

        public ParameterState State
        {
            get { return (ParameterState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // FormattedValue - форматированное значение для отображения
        public static readonly DependencyProperty FormattedValueProperty =
            DependencyProperty.Register(
                nameof(FormattedValue),
                typeof(string),
                typeof(ParameterDisplayControl),
                new PropertyMetadata(string.Empty));

        public string FormattedValue
        {
            get { return (string)GetValue(FormattedValueProperty); }
            private set { SetValue(FormattedValueProperty, value); }
        }

        // Callback при изменении Value или StringFormat - обновляем FormattedValue
        private static void OnValueOrFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ParameterDisplayControl control)
            {
                control.UpdateFormattedValue();
            }
        }

        private void UpdateFormattedValue()
        {
            if (Value == null)
            {
                FormattedValue = string.Empty;
                return;
            }

            try
            {
                if (!string.IsNullOrEmpty(StringFormat))
                {
                    // Применяем форматирование
                    FormattedValue = string.Format($"{{0:{StringFormat}}}", Value);
                }
                else
                {
                    FormattedValue = Value.ToString();
                }
            }
            catch
            {
                // Если форматирование не удалось, показываем как есть
                FormattedValue = Value.ToString();
            }
        }
    }
}
