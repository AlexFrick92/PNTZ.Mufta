using System.Windows;
using System.Windows.Media;

namespace PNTZ.Mufta.TPCApp.Styles
{
    /// <summary>
    /// Адаптер для доступа к цветам из Styles/AppColors.xaml
    /// Структура класса зеркально повторяет структуру XAML-файла
    /// </summary>
    public static class AppColors
    {
        // ==========================================
        // Графики: Границы допусков (ConstantLines)
        // ==========================================

        /// <summary>ChartLimitMin - минимальная граница допуска</summary>
        public static Brush ChartLimitMin => Get("ChartLimitMin");

        /// <summary>ChartLimitMax - максимальная граница допуска</summary>
        public static Brush ChartLimitMax => Get("ChartLimitMax");

        /// <summary>ChartLimitOptimal - оптимальное значение</summary>
        public static Brush ChartLimitOptimal => Get("ChartLimitOptimal");

        /// <summary>ChartLimitDump - сброс момента</summary>
        public static Brush ChartLimitDump => Get("ChartLimitDump");


        // ==========================================
        // Графики: Длина (Length)
        // ==========================================

        /// <summary>ChartLengthMin - минимальная длина</summary>
        public static Brush ChartLengthMin => Get("ChartLengthMin");

        /// <summary>ChartLengthMax - максимальная длина</summary>
        public static Brush ChartLengthMax => Get("ChartLengthMax");

        /// <summary>ChartLengthDump - сброс по длине</summary>
        public static Brush ChartLengthDump => Get("ChartLengthDump");


        // ==========================================
        // Графики: Буртик (Shoulder)
        // ==========================================

        /// <summary>ChartShoulderMin - минимальный момент буртика</summary>
        public static Brush ChartShoulderMin => Get("ChartShoulderMin");

        /// <summary>ChartShoulderMax - максимальный момент буртика</summary>
        public static Brush ChartShoulderMax => Get("ChartShoulderMax");


        // ==========================================
        // Графики: Выделенные области (Strips)
        // ==========================================

        /// <summary>ChartStripTorque - область допустимого момента</summary>
        public static Brush ChartStripTorque => Get("ChartStripTorque");

        /// <summary>ChartStripLength - область допустимой длины</summary>
        public static Brush ChartStripLength => Get("ChartStripLength");

        /// <summary>ChartStripShoulder - область допустимого момента буртика</summary>
        public static Brush ChartStripShoulder => Get("ChartStripShoulder");


        // ==========================================
        // Вспомогательный метод
        // ==========================================

        /// <summary>
        /// Получить кисть из ресурсов приложения по ключу
        /// </summary>
        /// <param name="key">Ключ ресурса</param>
        /// <returns>Кисть или Brushes.Black если ключ не найден</returns>
        private static Brush Get(string key)
        {
            return Application.Current.Resources[key] as Brush ?? Brushes.Black;
        }
    }
}
