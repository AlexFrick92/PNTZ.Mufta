using System;
using System.Windows;

namespace PNTZ.Mufta.TPCApp.View.Control.ValueTypes
{
    /// <summary>
    /// Тип значения для текста (string)
    /// </summary>
    public class TextValueType : ValueTypeBase
    {
        #region Dependency Properties

        // MinLength - минимальная допустимая длина строки
        public static readonly DependencyProperty MinLengthProperty =
            DependencyProperty.Register(
                nameof(MinLength),
                typeof(int?),
                typeof(TextValueType),
                new PropertyMetadata(null));

        public int? MinLength
        {
            get { return (int?)GetValue(MinLengthProperty); }
            set { SetValue(MinLengthProperty, value); }
        }

        // MaxLength - максимальная допустимая длина строки
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register(
                nameof(MaxLength),
                typeof(int?),
                typeof(TextValueType),
                new PropertyMetadata(null));

        public int? MaxLength
        {
            get { return (int?)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        #endregion

        public override object Parse(string text, Type targetType)
        {
            // Для текста парсинг не требуется - возвращаем как есть
            return text;
        }

        public override string Format(object value)
        {
            if (value == null)
                return string.Empty;

            return value.ToString();
        }

        public override ValidationResult Validate(object value)
        {
            if (!(value is string strValue))
                return ValidationResult.Success();

            int length = strValue.Length;

            // Проверка минимальной длины
            if (MinLength.HasValue && length < MinLength.Value)
            {
                if (MaxLength.HasValue)
                    return ValidationResult.Failure($"Длина должна быть от {MinLength.Value} до {MaxLength.Value} символов");
                else
                    return ValidationResult.Failure($"Длина должна быть не менее {MinLength.Value} символов");
            }

            // Проверка максимальной длины
            if (MaxLength.HasValue && length > MaxLength.Value)
            {
                if (MinLength.HasValue)
                    return ValidationResult.Failure($"Длина должна быть от {MinLength.Value} до {MaxLength.Value} символов");
                else
                    return ValidationResult.Failure($"Длина должна быть не более {MaxLength.Value} символов");
            }

            return ValidationResult.Success();
        }

        public override bool IsValidInput(string text)
        {
            // Для текста все символы допустимы
            return true;
        }
    }
}
