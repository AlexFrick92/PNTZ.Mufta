using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
    /// Тип валидации ввода
    /// </summary>
    public enum InputType
    {
        Text,      // без валидации
        Float,     // числа с точкой/запятой
        Integer    // только целые числа
    }

    /// <summary>
    /// Контрол для отображения параметра: Label + значение с цветовой индикацией состояния
    /// </summary>
    public partial class ParameterDisplayControl : UserControl
    {
        private bool _isUpdating = false; // Защита от циклических обновлений

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
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValueOrFormatChanged));

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

        // IsReadOnly - режим работы контрола (true = только чтение, false = ввод)
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                nameof(IsReadOnly),
                typeof(bool),
                typeof(ParameterDisplayControl),
                new PropertyMetadata(true));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        // InputType - тип валидации для режима ввода
        public static readonly DependencyProperty InputTypeProperty =
            DependencyProperty.Register(
                nameof(InputType),
                typeof(InputType),
                typeof(ParameterDisplayControl),
                new PropertyMetadata(InputType.Text));

        public InputType InputType
        {
            get { return (InputType)GetValue(InputTypeProperty); }
            set { SetValue(InputTypeProperty, value); }
        }

        // FormattedValue - форматированное значение для отображения
        public static readonly DependencyProperty FormattedValueProperty =
            DependencyProperty.Register(
                nameof(FormattedValue),
                typeof(string),
                typeof(ParameterDisplayControl),
                new PropertyMetadata(string.Empty, OnFormattedValueChanged));

        public string FormattedValue
        {
            get { return (string)GetValue(FormattedValueProperty); }
            set { SetValue(FormattedValueProperty, value); }
        }

        // Callback при изменении FormattedValue - парсим обратно в Value
        private static void OnFormattedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ParameterDisplayControl control && !control.IsReadOnly)
            {
                // В режиме редактирования парсим строку обратно в Value
                string formattedText = e.NewValue as string;
                if (!string.IsNullOrEmpty(formattedText))
                {
                    control.ParseFormattedValue(formattedText);
                }
            }
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
            if (_isUpdating) return; // Защита от циклических обновлений

            if (Value == null)
            {
                _isUpdating = true;
                FormattedValue = string.Empty;
                _isUpdating = false;
                return;
            }

            try
            {
                _isUpdating = true;

                if (!string.IsNullOrEmpty(StringFormat))
                {
                    // Если Value - строка, пытаемся распарсить как число
                    if (Value is string strValue)
                    {
                        if (double.TryParse(strValue, out double numValue))
                        {
                            // Успешно распарсили - применяем форматирование к числу
                            FormattedValue = string.Format($"{{0:{StringFormat}}}", numValue);
                        }
                        else
                        {
                            // Не число - показываем как есть (например, "Успех", "Брак")
                            FormattedValue = strValue;
                        }
                    }
                    else
                    {
                        // Value уже числовой тип - применяем форматирование напрямую
                        FormattedValue = string.Format($"{{0:{StringFormat}}}", Value);
                    }
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
            finally
            {
                _isUpdating = false;
            }
        }

        // Валидация ввода - вызывается из XAML через PreviewTextInput
        public void ValidateInput(object sender, TextCompositionEventArgs e)
        {
            switch (InputType)
            {
                case InputType.Float:
                    // Разрешаем цифры, точку, запятую и минус
                    e.Handled = !IsValidFloatInput(e.Text);
                    break;

                case InputType.Integer:
                    // Разрешаем только цифры и минус
                    e.Handled = !IsValidIntegerInput(e.Text);
                    break;

                case InputType.Text:
                default:
                    // Без валидации - всё разрешено
                    e.Handled = false;
                    break;
            }
        }

        private bool IsValidFloatInput(string text)
        {
            // Разрешаем: цифры, точку, запятую, минус
            return Regex.IsMatch(text, @"^[0-9\.\,\-]+$");
        }

        private bool IsValidIntegerInput(string text)
        {
            // Разрешаем: цифры и минус
            return Regex.IsMatch(text, @"^[0-9\-]+$");
        }

        // Парсинг FormattedValue обратно в Value
        private void ParseFormattedValue(string formattedText)
        {
            if (_isUpdating) return; // Защита от циклических обновлений

            try
            {
                _isUpdating = true;

                // Парсим в зависимости от InputType
                switch (InputType)
                {
                    case InputType.Integer:
                        // Парсим как int
                        if (int.TryParse(formattedText, out int intValue))
                        {
                            Value = intValue;
                        }
                        break;

                    case InputType.Float:
                        // Парсим как double
                        if (double.TryParse(formattedText, out double doubleValue))
                        {
                            Value = doubleValue;
                        }
                        break;

                    case InputType.Text:
                    default:
                        // Сохраняем как строку
                        Value = formattedText;
                        break;
                }
            }
            catch
            {
                // В случае ошибки парсинга оставляем Value без изменений
            }
            finally
            {
                _isUpdating = false;
            }
        }
    }
}
