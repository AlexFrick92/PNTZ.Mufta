using System.Windows;

namespace PNTZ.Mufta.TPCApp.Styles
{
    /// <summary>
    /// Адаптер для доступа к текстам лейблов из Styles/AppLabels.xaml
    /// Структура класса зеркально повторяет структуру XAML-файла
    /// </summary>
    public static class AppLabels
    {
        // ==========================================
        // Окно свинчивания (Joint Process)
        // ==========================================

        /// <summary>Заголовок секции "Текущие показания"</summary>
        public static string JointProcess_CurrentReadings => GetString("JointProcess_CurrentReadings");

        /// <summary>Заголовок секции "Результат"</summary>
        public static string JointProcess_Result => GetString("JointProcess_Result");

        /// <summary>Заголовок секции "Данные рецепта"</summary>
        public static string JointProcess_RecipeData => GetString("JointProcess_RecipeData");

        /// <summary>Оценка результата "Годная"</summary>
        public static string JointProcessResult_Good => GetString("JointProcessResult_Good");

        /// <summary>Оценка результата "Брак"</summary>
        public static string JointProcessResult_Bad => GetString("JointProcessResult_Bad");


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
        /// Получить строку из ресурсов приложения по ключу
        /// </summary>
        /// <param name="key">Ключ ресурса</param>
        /// <returns>Строка или "[key]" если ключ не найден</returns>
        private static string GetString(string key)
        {
            if (Application.Current.Resources[key] is string value)
                return value;
            return $"[{key}]"; // Возвращаем ключ в скобках, если не найден
        }
    }
}
