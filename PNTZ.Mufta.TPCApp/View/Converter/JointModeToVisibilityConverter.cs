using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace PNTZ.Mufta.TPCApp.View.Converter
{
    /// <summary>
    /// Конвертер для управления видимостью элементов в зависимости от режима JointMode.
    /// Поддерживает одно значение или массив значений через x:Array.
    /// </summary>
    public class JointModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            // Получаем текущий режим
            JointMode currentMode;
            if (value is JointMode mode)
            {
                currentMode = mode;
            }
            else
            {
                return Visibility.Collapsed;
            }

            // Проверяем, является ли parameter массивом
            if (parameter is Array array)
            {
                // Проверяем, содержится ли текущий режим в массиве
                foreach (var item in array)
                {
                    if (item is JointMode targetMode && currentMode == targetMode)
                        return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
            // Если передано одно значение
            else if (parameter is JointMode singleMode)
            {
                return currentMode == singleMode
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
