using System;
using System.Windows;

namespace PNTZ.Mufta.TPCApp.View.Control.ValueTypes
{
    /// <summary>
    /// Результат валидации значения
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public static ValidationResult Success() => new ValidationResult { IsValid = true };
        public static ValidationResult Failure(string message) => new ValidationResult { IsValid = false, ErrorMessage = message };
    }

    /// <summary>
    /// Базовый класс для типов значений с инкапсуляцией логики валидации, парсинга и форматирования
    /// </summary>
    public abstract class ValueTypeBase : DependencyObject
    {
        /// <summary>
        /// Парсинг строки в типизированное значение
        /// </summary>
        /// <param name="text">Текст для парсинга</param>
        /// <param name="targetType">Целевой тип для конвертации (например, ushort, int, double)</param>
        /// <returns>Распарсенное значение или null при ошибке</returns>
        public abstract object Parse(string text, Type targetType);

        /// <summary>
        /// Форматирование значения в строку для отображения
        /// </summary>
        /// <param name="value">Значение для форматирования</param>
        /// <returns>Отформатированная строка</returns>
        public abstract string Format(object value);

        /// <summary>
        /// Валидация значения (проверка диапазона, длины и т.д.)
        /// </summary>
        /// <param name="value">Значение для валидации</param>
        /// <returns>Результат валидации</returns>
        public abstract ValidationResult Validate(object value);

        /// <summary>
        /// Проверка допустимости ввода символа (для PreviewTextInput)
        /// </summary>
        /// <param name="text">Вводимый текст</param>
        /// <returns>true если ввод допустим</returns>
        public abstract bool IsValidInput(string text);

        /// <summary>
        /// Сравнение двух значений с учетом типа и форматирования
        /// </summary>
        /// <param name="value1">Первое значение</param>
        /// <param name="value2">Второе значение</param>
        /// <returns>true если значения равны</returns>
        public virtual bool AreValuesEqual(object value1, object value2)
        {
            // Если оба null - равны
            if (value1 == null && value2 == null)
                return true;

            // Если один null - не равны
            if (value1 == null || value2 == null)
                return false;

            // Сравниваем отформатированные строки
            string formatted1 = Format(value1);
            string formatted2 = Format(value2);

            return formatted1 == formatted2;
        }

        /// <summary>
        /// Конвертация значения в целевой тип (ushort, int, double и т.д.)
        /// </summary>
        protected object ConvertToTargetType(object value, Type targetType)
        {
            if (value == null || targetType == null)
                return value;

            try
            {
                // Если типы совпадают - возвращаем как есть
                if (value.GetType() == targetType)
                    return value;

                // Конвертируем в целевой тип
                if (targetType == typeof(ushort))
                    return Convert.ToUInt16(value);
                else if (targetType == typeof(short))
                    return Convert.ToInt16(value);
                else if (targetType == typeof(uint))
                    return Convert.ToUInt32(value);
                else if (targetType == typeof(int))
                    return Convert.ToInt32(value);
                else if (targetType == typeof(ulong))
                    return Convert.ToUInt64(value);
                else if (targetType == typeof(long))
                    return Convert.ToInt64(value);
                else if (targetType == typeof(byte))
                    return Convert.ToByte(value);
                else if (targetType == typeof(sbyte))
                    return Convert.ToSByte(value);
                else if (targetType == typeof(float))
                    return Convert.ToSingle(value);
                else if (targetType == typeof(double))
                    return Convert.ToDouble(value);
                else if (targetType == typeof(decimal))
                    return Convert.ToDecimal(value);
                else
                    return Convert.ChangeType(value, targetType);
            }
            catch
            {
                // Если конвертация не удалась, возвращаем исходное значение
                return value;
            }
        }
    }
}
