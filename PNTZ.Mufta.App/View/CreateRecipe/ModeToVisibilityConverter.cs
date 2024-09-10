using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

using PNTZ.Mufta.App.Domain.Joint;
using System.Windows;

namespace PNTZ.Mufta.App.View.CreateRecipe
{
    internal class ModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JointMode currentMode && parameter is JointMode buttonMode)
            {
                return currentMode == buttonMode ? Visibility.Visible : Visibility.Hidden;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
