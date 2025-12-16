using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace PNTZ.Mufta.TPCApp.View.Control.ValueTypes
{
    /// <summary>
    /// Тип значения для чисел с плавающей точкой (float, double, decimal)
    /// </summary>
    public class FloatValueType : ValueTypeBase
    {
        #region Dependency Properties

        // StringFormat - формат отображения (N0, F1, F2 и т.д.)
        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register(
                nameof(StringFormat),
                typeof(string),
                typeof(FloatValueType),
                new PropertyMetadata("F2"));

        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        // MinValue - минимальное допустимое значение
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(
                nameof(MinValue),
                typeof(double?),
                typeof(FloatValueType),
                new PropertyMetadata(null));

        public double? MinValue
        {
            get { return (double?)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        // MaxValue - максимальное допустимое значение
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                nameof(MaxValue),
                typeof(double?),
                typeof(FloatValueType),
                new PropertyMetadata(null));

        public double? MaxValue
        {
            get { return (double?)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        #endregion

        public override object Parse(string text, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            // Парсим как double
            if (double.TryParse(text, out double doubleValue))
            {
                // Конвертируем в целевой тип
                return ConvertToTargetType(doubleValue, targetType);
            }

            return null;
        }

        public override string Format(object value)
        {
            if (value == null)
                return string.Empty;

            try
            {
                // Если Value - строка, пытаемся распарсить как число
                if (value is string strValue)
                {
                    if (double.TryParse(strValue, out double numValue))
                    {
                        return string.Format($"{{0:{StringFormat}}}", numValue);
                    }
                    else
                    {
                        // Не число - показываем как есть
                        return strValue;
                    }
                }
                else
                {
                    // Value уже числовой тип - применяем форматирование напрямую
                    return string.Format($"{{0:{StringFormat}}}", value);
                }
            }
            catch
            {
                return value.ToString();
            }
        }

        public override ValidationResult Validate(object value)
        {
            if (value == null)
                return ValidationResult.Success();

            double doubleValue;

            // Конвертируем в double для проверки
            if (value is double d)
                doubleValue = d;
            else if (value is float f)
                doubleValue = f;
            else if (value is decimal dec)
                doubleValue = (double)dec;
            else if (value is int i)
                doubleValue = i;
            else
                return ValidationResult.Success(); // Не можем проверить

            // Проверка минимума
            if (MinValue.HasValue && doubleValue < MinValue.Value)
            {
                if (MaxValue.HasValue)
                    return ValidationResult.Failure($"Значение должно быть от {MinValue.Value} до {MaxValue.Value}");
                else
                    return ValidationResult.Failure($"Значение должно быть не менее {MinValue.Value}");
            }

            // Проверка максимума
            if (MaxValue.HasValue && doubleValue > MaxValue.Value)
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
            // Разрешаем: цифры, точку, запятую и минус
            return Regex.IsMatch(text, @"^[0-9\.\,\-]+$");
        }
    }
}
