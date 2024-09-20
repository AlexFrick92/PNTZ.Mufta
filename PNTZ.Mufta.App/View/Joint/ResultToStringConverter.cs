using System;
using System.Globalization;
using System.Windows.Data;

namespace PNTZ.Mufta.App.View.Joint
{
    public class ResultToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            if (value is uint result)
            {
                switch (result)
                {
                    case 0: return "Не установлен";
                    case 1: return "Успешно";
                    case 2: return "Неудачно";                        
                }                
            }
            return "Ошибка";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

