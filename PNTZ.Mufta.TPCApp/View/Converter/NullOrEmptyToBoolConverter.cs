using System;
using System.Globalization;
using System.Windows.Data;

namespace PNTZ.Mufta.TPCApp.View.Converter
{
    /// <summary>
    /// Конвертер, проверяющий значение на null или пустоту.
    /// null -> true
    /// пустая строка -> true
    /// любое другое значение -> false
    /// </summary>
    public class NullOrEmptyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return true;

            if (value is string str)
                return string.IsNullOrEmpty(str);

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
