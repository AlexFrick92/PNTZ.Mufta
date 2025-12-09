using System;
using System.Windows;

namespace PNTZ.Mufta.TPCApp.Styles
{
    /// <summary>
    /// Адаптер для доступа к настройкам из Styles/AppSettings.xaml
    /// Структура класса зеркально повторяет структуру XAML-файла
    /// </summary>
    public static class AppSettings
    {
        // ==========================================
        // Графики: Настройки отображения
        // ==========================================

        /// <summary>ChartMargin - отступ для границ графиков (по умолчанию 0.05 = 5%)</summary>
        public static double ChartMargin => GetDouble("ChartMargin");


        // ==========================================
        // Вспомогательные методы
        // ==========================================

        /// <summary>
        /// Получить значение типа Double из ресурсов приложения по ключу
        /// </summary>
        /// <param name="key">Ключ ресурса</param>
        /// <returns>Значение или 0.0 если ключ не найден</returns>
        private static double GetDouble(string key)
        {
            if (Application.Current.Resources[key] is double value)
            {
                return value;
            }
            return 0.0;
        }

        /// <summary>
        /// Получить значение типа Int32 из ресурсов приложения по ключу
        /// </summary>
        /// <param name="key">Ключ ресурса</param>
        /// <returns>Значение или 0 если ключ не найден</returns>
        private static int GetInt32(string key)
        {
            if (Application.Current.Resources[key] is int value)
            {
                return value;
            }
            return 0;
        }

        /// <summary>
        /// Получить значение типа String из ресурсов приложения по ключу
        /// </summary>
        /// <param name="key">Ключ ресурса</param>
        /// <returns>Значение или пустая строка если ключ не найден</returns>
        private static string GetString(string key)
        {
            if (Application.Current.Resources[key] is string value)
            {
                return value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Получить значение типа Boolean из ресурсов приложения по ключу
        /// </summary>
        /// <param name="key">Ключ ресурса</param>
        /// <returns>Значение или false если ключ не найден</returns>
        private static bool GetBoolean(string key)
        {
            if (Application.Current.Resources[key] is bool value)
            {
                return value;
            }
            return false;
        }
    }
}
