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
        private object _lastValidValue = null; // Последнее валидное значение для отката
        private Type _targetType = null; // Тип целевого свойства для правильной конвертации

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

        // MinValue - минимальное значение/длина (для Integer/Float - числовое, для Text - длина строки)
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(
                nameof(MinValue),
                typeof(object),
                typeof(ParameterDisplayControl),
                new PropertyMetadata(null));

        public object MinValue
        {
            get { return GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        // MaxValue - максимальное значение/длина (для Integer/Float - числовое, для Text - длина строки)
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                nameof(MaxValue),
                typeof(object),
                typeof(ParameterDisplayControl),
                new PropertyMetadata(null));

        public object MaxValue
        {
            get { return GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // IsValidationError - флаг ошибки валидации (приоритет над State)
        public static readonly DependencyProperty IsValidationErrorProperty =
            DependencyProperty.Register(
                nameof(IsValidationError),
                typeof(bool),
                typeof(ParameterDisplayControl),
                new PropertyMetadata(false));

        public bool IsValidationError
        {
            get { return (bool)GetValue(IsValidationErrorProperty); }
            set { SetValue(IsValidationErrorProperty, value); }
        }

        // ValidationErrorMessage - текст сообщения об ошибке валидации
        public static readonly DependencyProperty ValidationErrorMessageProperty =
            DependencyProperty.Register(
                nameof(ValidationErrorMessage),
                typeof(string),
                typeof(ParameterDisplayControl),
                new PropertyMetadata(string.Empty));

        public string ValidationErrorMessage
        {
            get { return (string)GetValue(ValidationErrorMessageProperty); }
            set { SetValue(ValidationErrorMessageProperty, value); }
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

                // Запоминаем тип целевого свойства при первой инициализации
                if (_targetType == null && Value != null)
                {
                    _targetType = Value.GetType();
                }

                // Сохраняем текущее значение как валидное (при программном обновлении)
                _lastValidValue = Value;
                IsValidationError = false; // Сбрасываем флаг ошибки при программном обновлении
                ValidationErrorMessage = string.Empty; // Очищаем сообщение об ошибке

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

        // Обработчик получения фокуса - выделяем весь текст
        private void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        // Обработчик клика мышкой - выделяем текст при первом клике
        private void OnTextBoxPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!textBox.IsKeyboardFocusWithin)
                {
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        // Обработчик потери фокуса - применяем форматирование к введенному значению
        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Принудительно обновляем биндинг, чтобы значение из TextBox.Text попало в FormattedValue
                var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
                bindingExpression?.UpdateSource();

                // После завершения ввода принудительно обновляем форматирование
                // если не было ошибки валидации
                if (!IsValidationError)
                {
                    UpdateFormattedValue();
                }
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

                object newValue = null;
                bool parseSuccess = false;

                // Парсим в зависимости от InputType
                switch (InputType)
                {
                    case InputType.Integer:
                        // Парсим как int и конвертируем в правильный тип
                        if (int.TryParse(formattedText, out int intValue))
                        {
                            newValue = ConvertToTargetType(intValue);
                            parseSuccess = true;
                        }
                        break;

                    case InputType.Float:
                        // Парсим как double и конвертируем в правильный тип
                        if (double.TryParse(formattedText, out double doubleValue))
                        {
                            newValue = ConvertToTargetType(doubleValue);
                            parseSuccess = true;
                        }
                        break;

                    case InputType.Text:
                    default:
                        // Сохраняем как строку
                        newValue = formattedText;
                        parseSuccess = true;
                        break;
                }

                // Проверяем диапазон и обновляем Value только если значение валидно
                if (parseSuccess && IsValueInRange(newValue))
                {
                    _lastValidValue = newValue; // Сохраняем последнее валидное значение
                    Value = newValue;
                    IsValidationError = false; // Сбрасываем флаг ошибки
                    ValidationErrorMessage = string.Empty; // Очищаем сообщение об ошибке
                }
                else
                {
                    // Значение невалидно - устанавливаем флаг ошибки
                    IsValidationError = true;
                    ValidationErrorMessage = GenerateValidationErrorMessage();

                    if (_lastValidValue != null)
                    {
                        // Откатываемся к последнему валидному значению
                        Value = _lastValidValue;
                    }
                }
            }
            catch
            {
                // В случае ошибки парсинга устанавливаем флаг ошибки и откатываемся
                IsValidationError = true;
                ValidationErrorMessage = "Некорректный формат ввода";
                if (_lastValidValue != null)
                {
                    Value = _lastValidValue;
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        // Проверка значения на попадание в диапазон Min/Max
        private bool IsValueInRange(object value)
        {
            if (value == null) return true;

            switch (InputType)
            {
                case InputType.Integer:
                    return IsIntegerInRange(value);

                case InputType.Float:
                    return IsFloatInRange(value);

                case InputType.Text:
                    return IsTextLengthInRange(value);

                default:
                    return true;
            }
        }

        private bool IsIntegerInRange(object value)
        {
            if (!(value is int intValue)) return true;

            // Проверка минимума
            if (MinValue != null)
            {
                int? minInt = ParseAsInt(MinValue);
                if (minInt.HasValue && intValue < minInt.Value) return false;
            }

            // Проверка максимума
            if (MaxValue != null)
            {
                int? maxInt = ParseAsInt(MaxValue);
                if (maxInt.HasValue && intValue > maxInt.Value) return false;
            }

            return true;
        }

        private int? ParseAsInt(object obj)
        {
            if (obj == null) return null;
            if (obj is int i) return i;
            if (obj is string str && int.TryParse(str, out int parsed)) return parsed;
            if (obj is double d) return (int)d;
            return null;
        }

        private bool IsFloatInRange(object value)
        {
            double doubleValue;
            if (value is double d)
                doubleValue = d;
            else if (value is float f)
                doubleValue = f;
            else if (value is int i)
                doubleValue = i;
            else
                return true;

            // Проверка минимума
            if (MinValue != null)
            {
                double? minDouble = ParseAsDouble(MinValue);
                if (minDouble.HasValue && doubleValue < minDouble.Value) return false;
            }

            // Проверка максимума
            if (MaxValue != null)
            {
                double? maxDouble = ParseAsDouble(MaxValue);
                if (maxDouble.HasValue && doubleValue > maxDouble.Value) return false;
            }

            return true;
        }

        private double? ParseAsDouble(object obj)
        {
            if (obj == null) return null;
            if (obj is double d) return d;
            if (obj is float f) return f;
            if (obj is int i) return i;
            if (obj is string str && double.TryParse(str, out double parsed)) return parsed;
            return null;
        }

        private bool IsTextLengthInRange(object value)
        {
            if (!(value is string strValue)) return true;

            int length = strValue.Length;

            // Проверка минимальной длины
            if (MinValue != null)
            {
                int? minLength = ParseAsInt(MinValue);
                if (minLength.HasValue && length < minLength.Value) return false;
            }

            // Проверка максимальной длины
            if (MaxValue != null)
            {
                int? maxLength = ParseAsInt(MaxValue);
                if (maxLength.HasValue && length > maxLength.Value) return false;
            }

            return true;
        }

        // Генерация текста сообщения об ошибке валидации
        private string GenerateValidationErrorMessage()
        {
            switch (InputType)
            {
                case InputType.Integer:
                case InputType.Float:
                    return GenerateNumericRangeMessage();

                case InputType.Text:
                    return GenerateTextLengthMessage();

                default:
                    return "Некорректное значение";
            }
        }

        private string GenerateNumericRangeMessage()
        {
            int? minInt = ParseAsInt(MinValue);
            int? maxInt = ParseAsInt(MaxValue);
            double? minDouble = ParseAsDouble(MinValue);
            double? maxDouble = ParseAsDouble(MaxValue);

            if (InputType == InputType.Integer)
            {
                if (minInt.HasValue && maxInt.HasValue)
                    return $"Значение должно быть от {minInt.Value} до {maxInt.Value}";
                else if (minInt.HasValue)
                    return $"Значение должно быть не менее {minInt.Value}";
                else if (maxInt.HasValue)
                    return $"Значение должно быть не более {maxInt.Value}";
            }
            else // Float
            {
                if (minDouble.HasValue && maxDouble.HasValue)
                    return $"Значение должно быть от {minDouble.Value} до {maxDouble.Value}";
                else if (minDouble.HasValue)
                    return $"Значение должно быть не менее {minDouble.Value}";
                else if (maxDouble.HasValue)
                    return $"Значение должно быть не более {maxDouble.Value}";
            }

            return "Некорректное значение";
        }

        private string GenerateTextLengthMessage()
        {
            int? minLength = ParseAsInt(MinValue);
            int? maxLength = ParseAsInt(MaxValue);

            if (minLength.HasValue && maxLength.HasValue)
                return $"Длина должна быть от {minLength.Value} до {maxLength.Value} символов";
            else if (minLength.HasValue)
                return $"Длина должна быть не менее {minLength.Value} символов";
            else if (maxLength.HasValue)
                return $"Длина должна быть не более {maxLength.Value} символов";

            return "Некорректная длина строки";
        }

        // Конвертирует значение в тип целевого свойства
        private object ConvertToTargetType(object value)
        {
            if (value == null || _targetType == null)
                return value;

            try
            {
                // Если типы совпадают - возвращаем как есть
                if (value.GetType() == _targetType)
                    return value;

                // Конвертируем в целевой тип
                if (_targetType == typeof(ushort))
                    return Convert.ToUInt16(value);
                else if (_targetType == typeof(short))
                    return Convert.ToInt16(value);
                else if (_targetType == typeof(uint))
                    return Convert.ToUInt32(value);
                else if (_targetType == typeof(int))
                    return Convert.ToInt32(value);
                else if (_targetType == typeof(ulong))
                    return Convert.ToUInt64(value);
                else if (_targetType == typeof(long))
                    return Convert.ToInt64(value);
                else if (_targetType == typeof(byte))
                    return Convert.ToByte(value);
                else if (_targetType == typeof(sbyte))
                    return Convert.ToSByte(value);
                else if (_targetType == typeof(float))
                    return Convert.ToSingle(value);
                else if (_targetType == typeof(double))
                    return Convert.ToDouble(value);
                else if (_targetType == typeof(decimal))
                    return Convert.ToDecimal(value);
                else
                    return Convert.ChangeType(value, _targetType);
            }
            catch
            {
                // Если конвертация не удалась, возвращаем исходное значение
                return value;
            }
        }
    }
}
