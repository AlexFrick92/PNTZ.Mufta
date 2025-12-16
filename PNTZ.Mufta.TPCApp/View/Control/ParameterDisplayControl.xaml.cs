using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PNTZ.Mufta.TPCApp.View.Control.ValueTypes;

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
        private bool _isUpdating = false; // Защита от циклических обновлений
        private Type _targetType = null; // Тип целевого свойства для правильной конвертации

        #region Dep-properties
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

        // ValueType - новый способ конфигурации типа значения
        public static readonly DependencyProperty ValueTypeProperty =
            DependencyProperty.Register(
                nameof(ValueType),
                typeof(ValueTypeBase),
                typeof(ParameterDisplayControl),
                new PropertyMetadata(null, OnValueTypeChanged));

        public ValueTypeBase ValueType
        {
            get { return (ValueTypeBase)GetValue(ValueTypeProperty); }
            set { SetValue(ValueTypeProperty, value); }
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
        #endregion

        public ParameterDisplayControl()
        {
            InitializeComponent();
            Validation.AddErrorHandler(this, OnValidationError);
        }

        private void OnValidationError(object sender, ValidationErrorEventArgs e)
        {
            // Обрабатываем через Dispatcher, чтобы UI обновился корректно
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (e.Action == ValidationErrorEventAction.Added)
                {
                    IsValidationError = true;
                    ValidationErrorMessage = e.Error.ErrorContent?.ToString() ?? "Ошибка присвоения значения";
                }
                else
                {
                    IsValidationError = false;
                    ValidationErrorMessage = string.Empty;
                }
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        // Callback при изменении ValueType - обновляем форматирование
        private static void OnValueTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ParameterDisplayControl control)
            {
                control.UpdateFormattedValue();
            }
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

                // Сбрасываем флаг ошибки при программном обновлении
                IsValidationError = false;
                ValidationErrorMessage = string.Empty;

                // Используем ValueType для форматирования, если он есть
                if (ValueType != null)
                {
                    FormattedValue = ValueType.Format(Value);
                }
                else
                {
                    // Fallback на старую логику для backward compatibility
                    FormattedValue = Value.ToString();
                }
            }
            catch
            {
                // Если форматирование не удалось, показываем как есть
                FormattedValue = Value?.ToString() ?? string.Empty;
            }
            finally
            {
                _isUpdating = false;
            }
        }

        // Валидация ввода - вызывается из XAML через PreviewTextInput
        public void ValidateInput(object sender, TextCompositionEventArgs e)
        {
            // Используем ValueType для валидации, если он есть
            if (ValueType != null)
            {
                e.Handled = !ValueType.IsValidInput(e.Text);
            }
            else
            {
                // Fallback - разрешаем всё
                e.Handled = false;
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

        // Парсинг FormattedValue обратно в Value
        private void ParseFormattedValue(string formattedText)
        {
            if (_isUpdating) return; // Защита от циклических обновлений

            try
            {
                _isUpdating = true;

                // Используем ValueType для парсинга и валидации
                if (ValueType != null)
                {
                    object parsedValue = ValueType.Parse(formattedText, _targetType);

                    if (parsedValue != null)
                    {
                        // Валидируем значение
                        var validationResult = ValueType.Validate(parsedValue);

                        if (validationResult.IsValid)
                        {
                            Value = parsedValue;
                            IsValidationError = false;
                            ValidationErrorMessage = string.Empty;
                        }
                        else
                        {
                            // Значение невалидно - устанавливаем флаг ошибки
                            IsValidationError = true;
                            ValidationErrorMessage = validationResult.ErrorMessage;
                        }
                    }
                    else
                    {
                        // Парсинг не удался
                        IsValidationError = true;
                        ValidationErrorMessage = "Некорректный формат ввода";
                    }
                }
                else
                {
                    // Fallback - просто присваиваем текст
                    Value = formattedText;
                    IsValidationError = false;
                    ValidationErrorMessage = string.Empty;
                }
            }
            catch
            {
                // В случае ошибки парсинга устанавливаем флаг ошибки
                IsValidationError = true;
                ValidationErrorMessage = "Некорректный формат ввода";
            }
            finally
            {
                _isUpdating = false;
            }
        }
    }
}
