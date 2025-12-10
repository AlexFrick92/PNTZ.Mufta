using System;
using System.Globalization;
using System.Windows.Data;
using PNTZ.Mufta.TPCApp.Styles;

namespace PNTZ.Mufta.TPCApp.View.Converter
{
    /// <summary>
    /// Конвертер, преобразующий uint значение в строковое представление.
    /// </summary>
    public class ResultTotalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is uint uintValue)
            {
                switch (uintValue)
                {
                    case 0:
                        return "";
                    case 1:
                        return AppLabels.JointProcessResult_Good;
                    case 2:
                        return AppLabels.JointProcessResult_Bad;
                    default:
                        return uintValue.ToString();
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
