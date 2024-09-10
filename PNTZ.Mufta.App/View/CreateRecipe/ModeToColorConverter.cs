
using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

using PNTZ.Mufta.App.Domain.Joint;

namespace PNTZ.Mufta.App.View.CreateRecipe
{
    public class ModeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JointMode currentMode && parameter is JointMode buttonMode)
            {
                return currentMode == buttonMode ? Brushes.LightGreen : Brushes.Gray;
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
