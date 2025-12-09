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

        /// <summary>ChartLimitMin_Line - минимальная граница допуска (линия)</summary>
        public static Brush ChartLimitMin_Line => Get("ChartLimitMin_Line");

        /// <summary>ChartLimitMin_Label - минимальная граница допуска (лейбл)</summary>
        public static Brush ChartLimitMin_Label => Get("ChartLimitMin_Label");

        /// <summary>ChartLimitMax_Line - максимальная граница допуска (линия)</summary>
        public static Brush ChartLimitMax_Line => Get("ChartLimitMax_Line");

        /// <summary>ChartLimitMax_Label - максимальная граница допуска (лейбл)</summary>
        public static Brush ChartLimitMax_Label => Get("ChartLimitMax_Label");

        /// <summary>ChartLimitOptimal_Line - оптимальное значение (линия)</summary>
        public static Brush ChartLimitOptimal_Line => Get("ChartLimitOptimal_Line");

        /// <summary>ChartLimitOptimal_Label - оптимальное значение (лейбл)</summary>
        public static Brush ChartLimitOptimal_Label => Get("ChartLimitOptimal_Label");

        /// <summary>ChartLimitDump_Line - сброс момента (линия)</summary>
        public static Brush ChartLimitDump_Line => Get("ChartLimitDump_Line");

        /// <summary>ChartLimitDump_Label - сброс момента (лейбл)</summary>
        public static Brush ChartLimitDump_Label => Get("ChartLimitDump_Label");


        // ==========================================
        // Графики: Длина (Length)
        // ==========================================

        /// <summary>ChartLengthMin_Line - минимальная длина (линия)</summary>
        public static Brush ChartLengthMin_Line => Get("ChartLengthMin_Line");

        /// <summary>ChartLengthMin_Label - минимальная длина (лейбл)</summary>
        public static Brush ChartLengthMin_Label => Get("ChartLengthMin_Label");

        /// <summary>ChartLengthMax_Line - максимальная длина (линия)</summary>
        public static Brush ChartLengthMax_Line => Get("ChartLengthMax_Line");

        /// <summary>ChartLengthMax_Label - максимальная длина (лейбл)</summary>
        public static Brush ChartLengthMax_Label => Get("ChartLengthMax_Label");

        /// <summary>ChartLengthDump_Line - сброс по длине (линия)</summary>
        public static Brush ChartLengthDump_Line => Get("ChartLengthDump_Line");

        /// <summary>ChartLengthDump_Label - сброс по длине (лейбл)</summary>
        public static Brush ChartLengthDump_Label => Get("ChartLengthDump_Label");


        // ==========================================
        // Графики: Буртик (Shoulder)
        // ==========================================

        /// <summary>ChartShoulderMin_Line - минимальный момент буртика (линия)</summary>
        public static Brush ChartShoulderMin_Line => Get("ChartShoulderMin_Line");

        /// <summary>ChartShoulderMin_Label - минимальный момент буртика (лейбл)</summary>
        public static Brush ChartShoulderMin_Label => Get("ChartShoulderMin_Label");

        /// <summary>ChartShoulderMax_Line - максимальный момент буртика (линия)</summary>
        public static Brush ChartShoulderMax_Line => Get("ChartShoulderMax_Line");

        /// <summary>ChartShoulderMax_Label - максимальный момент буртика (лейбл)</summary>
        public static Brush ChartShoulderMax_Label => Get("ChartShoulderMax_Label");


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
