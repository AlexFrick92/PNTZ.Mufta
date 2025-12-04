using System;
using System.Globalization;
using System.Windows.Data;

namespace PNTZ.Mufta.TPCApp.View.Formatter
{
    /// <summary>
    /// Конвертер для форматирования double значений с использованием указанного формата.
    /// Работает как с IValueConverter (формат через parameter), так и с IMultiValueConverter (формат через второе значение).
    /// </summary>
    public class DoubleFormatConverter : IValueConverter, IMultiValueConverter
    {
        // IValueConverter - используется когда формат передается через ConverterParameter
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue && parameter is string format)
            {
                return doubleValue.ToString(format);
            }
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        // IMultiValueConverter - используется когда формат передается через Binding
        // values[0] = Value (double), values[1] = ValueFormat (string)
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is double doubleValue && values[1] is string format)
            {
                return doubleValue.ToString(format);
            }
            return values[0]?.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
