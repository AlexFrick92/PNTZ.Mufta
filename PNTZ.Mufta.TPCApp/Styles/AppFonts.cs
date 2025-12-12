using System.Windows;
using System.Windows.Media;

namespace PNTZ.Mufta.TPCApp.Styles
{
    /// <summary>
    /// Адаптер для доступа к шрифтам из Styles/AppFonts.xaml
    /// Структура класса зеркально повторяет структуру XAML-файла
    /// </summary>
    public static class AppFonts
    {
        // ==========================================
        // Графики: Границы допусков (ConstantLines)
        // ==========================================

        /// <summary>ChartLimitMin - минимальная граница допуска (размер шрифта)</summary>
        public static double ChartLimitMin_FontSize => GetDouble("ChartLimitMin_FontSize");

        /// <summary>ChartLimitMin - минимальная граница допуска (толщина шрифта)</summary>
        public static FontWeight ChartLimitMin_FontWeight => GetFontWeight("ChartLimitMin_FontWeight");

        /// <summary>ChartLimitMin - минимальная граница допуска (семейство шрифта)</summary>
        public static FontFamily ChartLimitMin_FontFamily => GetFontFamily("ChartLimitMin_FontFamily");


        /// <summary>ChartLimitMax - максимальная граница допуска (размер шрифта)</summary>
        public static double ChartLimitMax_FontSize => GetDouble("ChartLimitMax_FontSize");

        /// <summary>ChartLimitMax - максимальная граница допуска (толщина шрифта)</summary>
        public static FontWeight ChartLimitMax_FontWeight => GetFontWeight("ChartLimitMax_FontWeight");

        /// <summary>ChartLimitMax - максимальная граница допуска (семейство шрифта)</summary>
        public static FontFamily ChartLimitMax_FontFamily => GetFontFamily("ChartLimitMax_FontFamily");


        /// <summary>ChartLimitOptimal - оптимальное значение (размер шрифта)</summary>
        public static double ChartLimitOptimal_FontSize => GetDouble("ChartLimitOptimal_FontSize");

        /// <summary>ChartLimitOptimal - оптимальное значение (толщина шрифта)</summary>
        public static FontWeight ChartLimitOptimal_FontWeight => GetFontWeight("ChartLimitOptimal_FontWeight");

        /// <summary>ChartLimitOptimal - оптимальное значение (семейство шрифта)</summary>
        public static FontFamily ChartLimitOptimal_FontFamily => GetFontFamily("ChartLimitOptimal_FontFamily");


        /// <summary>ChartLimitDump - сброс момента (размер шрифта)</summary>
        public static double ChartLimitDump_FontSize => GetDouble("ChartLimitDump_FontSize");

        /// <summary>ChartLimitDump - сброс момента (толщина шрифта)</summary>
        public static FontWeight ChartLimitDump_FontWeight => GetFontWeight("ChartLimitDump_FontWeight");

        /// <summary>ChartLimitDump - сброс момента (семейство шрифта)</summary>
        public static FontFamily ChartLimitDump_FontFamily => GetFontFamily("ChartLimitDump_FontFamily");


        // ==========================================
        // Графики: Длина (Length)
        // ==========================================

        /// <summary>ChartLengthMin - минимальная длина (размер шрифта)</summary>
        public static double ChartLengthMin_FontSize => GetDouble("ChartLengthMin_FontSize");

        /// <summary>ChartLengthMin - минимальная длина (толщина шрифта)</summary>
        public static FontWeight ChartLengthMin_FontWeight => GetFontWeight("ChartLengthMin_FontWeight");

        /// <summary>ChartLengthMin - минимальная длина (семейство шрифта)</summary>
        public static FontFamily ChartLengthMin_FontFamily => GetFontFamily("ChartLengthMin_FontFamily");


        /// <summary>ChartLengthMax - максимальная длина (размер шрифта)</summary>
        public static double ChartLengthMax_FontSize => GetDouble("ChartLengthMax_FontSize");

        /// <summary>ChartLengthMax - максимальная длина (толщина шрифта)</summary>
        public static FontWeight ChartLengthMax_FontWeight => GetFontWeight("ChartLengthMax_FontWeight");

        /// <summary>ChartLengthMax - максимальная длина (семейство шрифта)</summary>
        public static FontFamily ChartLengthMax_FontFamily => GetFontFamily("ChartLengthMax_FontFamily");


        /// <summary>ChartLengthDump - сброс по длине (размер шрифта)</summary>
        public static double ChartLengthDump_FontSize => GetDouble("ChartLengthDump_FontSize");

        /// <summary>ChartLengthDump - сброс по длине (толщина шрифта)</summary>
        public static FontWeight ChartLengthDump_FontWeight => GetFontWeight("ChartLengthDump_FontWeight");

        /// <summary>ChartLengthDump - сброс по длине (семейство шрифта)</summary>
        public static FontFamily ChartLengthDump_FontFamily => GetFontFamily("ChartLengthDump_FontFamily");


        // ==========================================
        // Графики: Буртик (Shoulder)
        // ==========================================

        /// <summary>ChartShoulderMin - минимальный момент буртика (размер шрифта)</summary>
        public static double ChartShoulderMin_FontSize => GetDouble("ChartShoulderMin_FontSize");

        /// <summary>ChartShoulderMin - минимальный момент буртика (толщина шрифта)</summary>
        public static FontWeight ChartShoulderMin_FontWeight => GetFontWeight("ChartShoulderMin_FontWeight");

        /// <summary>ChartShoulderMin - минимальный момент буртика (семейство шрифта)</summary>
        public static FontFamily ChartShoulderMin_FontFamily => GetFontFamily("ChartShoulderMin_FontFamily");


        /// <summary>ChartShoulderMax - максимальный момент буртика (размер шрифта)</summary>
        public static double ChartShoulderMax_FontSize => GetDouble("ChartShoulderMax_FontSize");

        /// <summary>ChartShoulderMax - максимальный момент буртика (толщина шрифта)</summary>
        public static FontWeight ChartShoulderMax_FontWeight => GetFontWeight("ChartShoulderMax_FontWeight");

        /// <summary>ChartShoulderMax - максимальный момент буртика (семейство шрифта)</summary>
        public static FontFamily ChartShoulderMax_FontFamily => GetFontFamily("ChartShoulderMax_FontFamily");


        // ==========================================
        // Графики: Заголовки осей
        // ==========================================

        /// <summary>ChartAxisX - заголовок оси X (размер шрифта)</summary>
        public static double ChartAxisX_FontSize => GetDouble("ChartAxisX_FontSize");

        /// <summary>ChartAxisX - заголовок оси X (толщина шрифта)</summary>
        public static FontWeight ChartAxisX_FontWeight => GetFontWeight("ChartAxisX_FontWeight");

        /// <summary>ChartAxisX - заголовок оси X (семейство шрифта)</summary>
        public static FontFamily ChartAxisX_FontFamily => GetFontFamily("ChartAxisX_FontFamily");


        /// <summary>ChartAxisY - заголовок оси Y (размер шрифта)</summary>
        public static double ChartAxisY_FontSize => GetDouble("ChartAxisY_FontSize");

        /// <summary>ChartAxisY - заголовок оси Y (толщина шрифта)</summary>
        public static FontWeight ChartAxisY_FontWeight => GetFontWeight("ChartAxisY_FontWeight");

        /// <summary>ChartAxisY - заголовок оси Y (семейство шрифта)</summary>
        public static FontFamily ChartAxisY_FontFamily => GetFontFamily("ChartAxisY_FontFamily");


        // ==========================================
        // Окно процесса свинчивания
        // ==========================================

        /// <summary>JointProcess_ParamHeader - заголовок параметра (размер шрифта)</summary>
        public static double JointProcess_ParamHeader_FontSize => GetDouble("JointProcess_ParamHeader_FontSize");

        /// <summary>JointProcess_ParamHeader - заголовок параметра (толщина шрифта)</summary>
        public static FontWeight JointProcess_ParamHeader_FontWeight => GetFontWeight("JointProcess_ParamHeader_FontWeight");

        /// <summary>JointProcess_ParamHeader - заголовок параметра (семейство шрифта)</summary>
        public static FontFamily JointProcess_ParamHeader_FontFamily => GetFontFamily("JointProcess_ParamHeader_FontFamily");


        // ==========================================
        // Контрол отображения параметра
        // ==========================================

        /// <summary>ParameterDisplay_Label - метка параметра (размер шрифта)</summary>
        public static double ParameterDisplay_Label_FontSize => GetDouble("ParameterDisplay_Label_FontSize");

        /// <summary>ParameterDisplay_Label - метка параметра (толщина шрифта)</summary>
        public static FontWeight ParameterDisplay_Label_FontWeight => GetFontWeight("ParameterDisplay_Label_FontWeight");

        /// <summary>ParameterDisplay_Label - метка параметра (семейство шрифта)</summary>
        public static FontFamily ParameterDisplay_Label_FontFamily => GetFontFamily("ParameterDisplay_Label_FontFamily");


        /// <summary>ParameterDisplay_Value - значение параметра (размер шрифта)</summary>
        public static double ParameterDisplay_Value_FontSize => GetDouble("ParameterDisplay_Value_FontSize");

        /// <summary>ParameterDisplay_Value - значение параметра (толщина шрифта)</summary>
        public static FontWeight ParameterDisplay_Value_FontWeight => GetFontWeight("ParameterDisplay_Value_FontWeight");

        /// <summary>ParameterDisplay_Value - значение параметра (семейство шрифта)</summary>
        public static FontFamily ParameterDisplay_Value_FontFamily => GetFontFamily("ParameterDisplay_Value_FontFamily");


        // ==========================================
        // Окно процесса: Заголовки секций
        // ==========================================

        /// <summary>JointView_SectionHeader - заголовок секции (размер шрифта)</summary>
        public static double JointView_SectionHeader_FontSize => GetDouble("JointView_SectionHeader_FontSize");

        /// <summary>JointView_SectionHeader - заголовок секции (толщина шрифта)</summary>
        public static FontWeight JointView_SectionHeader_FontWeight => GetFontWeight("JointView_SectionHeader_FontWeight");

        /// <summary>JointView_SectionHeader - заголовок секции (семейство шрифта)</summary>
        public static FontFamily JointView_SectionHeader_FontFamily => GetFontFamily("JointView_SectionHeader_FontFamily");


        // ==========================================
        // Вспомогательные методы
        // ==========================================

        /// <summary>
        /// Получить значение double из ресурсов приложения по ключу
        /// </summary>
        /// <param name="key">Ключ ресурса</param>
        /// <returns>Значение или 12.0 если ключ не найден</returns>
        private static double GetDouble(string key)
        {
            if (Application.Current.Resources[key] is double value)
                return value;
            return 12.0; // Default font size
        }

        /// <summary>
        /// Получить FontWeight из ресурсов приложения по ключу
        /// </summary>
        /// <param name="key">Ключ ресурса</param>
        /// <returns>FontWeight или FontWeights.Normal если ключ не найден</returns>
        private static FontWeight GetFontWeight(string key)
        {
            if (Application.Current.Resources[key] is FontWeight value)
                return value;
            return FontWeights.Normal;
        }

        /// <summary>
        /// Получить FontFamily из ресурсов приложения по ключу
        /// </summary>
        /// <param name="key">Ключ ресурса</param>
        /// <returns>FontFamily или "Segoe UI" если ключ не найден</returns>
        private static FontFamily GetFontFamily(string key)
        {
            if (Application.Current.Resources[key] is FontFamily value)
                return value;
            return new FontFamily("Segoe UI");
        }
    }
}
