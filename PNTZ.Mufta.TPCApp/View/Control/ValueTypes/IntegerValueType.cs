using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace PNTZ.Mufta.TPCApp.View.Control.ValueTypes
{
    /// <summary>
    /// Тип значения для целых чисел (int, uint, short, ushort, long, ulong, byte, sbyte)
    /// </summary>
    public class IntegerValueType : ValueTypeBase
    {
        #region Dependency Properties

        // MinValue - минимальное допустимое значение
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(
                nameof(MinValue),
                typeof(int?),
                typeof(IntegerValueType),
                new PropertyMetadata(null));

        public int? MinValue
        {
            get { return (int?)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        // MaxValue - максимальное допустимое значение
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                nameof(MaxValue),
                typeof(int?),
                typeof(IntegerValueType),
                new PropertyMetadata(null));

        public int? MaxValue
        {
            get { return (int?)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        #endregion

        public override object Parse(string text, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            // Парсим как int
            if (int.TryParse(text, out int intValue))
            {
                // Конвертируем в целевой тип
                return ConvertToTargetType(intValue, targetType);
            }

            return null;
        }

        public override string Format(object value)
        {
            if (value == null)
                return string.Empty;

            return value.ToString();
        }

        public override ValidationResult Validate(object value)
        {
            if (value == null)
                return ValidationResult.Success();

            int intValue;

            // Конвертируем в int для проверки
            if (value is int i)
                intValue = i;
            else if (value is uint ui)
                intValue = (int)ui;
            else if (value is short s)
                intValue = s;
            else if (value is ushort us)
                intValue = us;
            else if (value is byte b)
                intValue = b;
            else if (value is sbyte sb)
                intValue = sb;
            else if (value is long l)
                intValue = (int)l;
            else if (value is ulong ul)
                intValue = (int)ul;
            else
                return ValidationResult.Success(); // Не можем проверить

            // Проверка минимума
            if (MinValue.HasValue && intValue < MinValue.Value)
            {
                if (MaxValue.HasValue)
                    return ValidationResult.Failure($"Значение должно быть от {MinValue.Value} до {MaxValue.Value}");
                else
                    return ValidationResult.Failure($"Значение должно быть не менее {MinValue.Value}");
            }

            // Проверка максимума
            if (MaxValue.HasValue && intValue > MaxValue.Value)
            {
                if (MinValue.HasValue)
                    return ValidationResult.Failure($"Значение должно быть от {MinValue.Value} до {MaxValue.Value}");
                else
                    return ValidationResult.Failure($"Значение должно быть не более {MaxValue.Value}");
            }

            return ValidationResult.Success();
        }

        public override bool IsValidInput(string text)
        {
            // Разрешаем: цифры и минус
            return Regex.IsMatch(text, @"^[0-9\-]+$");
        }
    }
}
