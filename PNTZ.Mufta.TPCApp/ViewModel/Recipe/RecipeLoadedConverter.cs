using System;
using System.Globalization;
using System.Windows.Data;

namespace PNTZ.Mufta.TPCApp.ViewModel.Recipe
{
    /// <summary>
    /// Converter для определения, является ли рецепт загруженным
    /// </summary>
    public class RecipeLoadedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return false;

            // values[0] - текущий рецепт (из DataContext элемента)
            // values[1] - LoadedRecipe из ViewModel
            return values[0] != null && values[0] == values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
