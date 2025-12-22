using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PNTZ.Mufta.TPCApp.ViewModel.Recipe
{
    /// <summary>
    /// Converter для преобразования JointMode enum в читаемый текст
    /// </summary>
    public class JointModeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JointMode jointMode)
            {
                switch (jointMode)
                {
                    case JointMode.Torque:
                        return "💪 По моменту";

                    case JointMode.TorqueShoulder:
                        return "💪 По моменту с контролем заплечника";

                    case JointMode.Length:
                        return "📐 По глубине";

                    case JointMode.TorqueLength:
                        return "💪📐 По глубине с контролем момента";

                    case JointMode.Jval:
                        return "📏 По значению J";

                    case JointMode.TorqueJVal:
                        return "💪📏 По значению J с контролем момента";

                    default:
                        return "не выбран";
                }
            }

            return "не выбран";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
