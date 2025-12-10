using System.Windows;

namespace PNTZ.Mufta.TPCApp.Styles
{
    /// <summary>
    /// Адаптер для доступа к настройкам разметки из Styles/AppLayouts.xaml
    /// Структура класса зеркально повторяет структуру XAML-файла
    /// </summary>
    public static class AppLayouts
    {
        // ==========================================
        // Окно свинчивания (Joint Process)
        // ==========================================

        /// <summary>Ширина столбца значений в ParameterDisplayControl</summary>
        public static GridLength ParameterDisplay_ValueColumnWidth => GetGridLength("ParameterDisplay_ValueColumnWidth");


        // ==========================================
        // Окно рецепта (Recipe)
        // ==========================================

        // TODO: Добавить свойства для окна рецепта


        // ==========================================
        // Окно результата (Results)
        // ==========================================

        // TODO: Добавить свойства для окна результата


        // ==========================================
        // Вспомогательные методы
        // ==========================================

        /// <summary>
        /// Получить GridLength из ресурсов приложения по ключу
        /// </summary>
        /// <param name="key">Ключ ресурса</param>
        /// <returns>GridLength или GridLength(100) если ключ не найден</returns>
        private static GridLength GetGridLength(string key)
        {
            if (Application.Current.Resources[key] is GridLength value)
                return value;
            return new GridLength(100); // Default width
        }
    }
}
