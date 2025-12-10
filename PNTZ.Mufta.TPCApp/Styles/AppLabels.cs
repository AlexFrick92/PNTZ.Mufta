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

        /// <summary>Лейбл параметра "Момент, Нм" (текущие показания)</summary>
        public static string JointProcess_Torque_Label => GetString("JointProcess_Torque_Label");

        /// <summary>Лейбл параметра "Длина, мм" (текущие показания)</summary>
        public static string JointProcess_Length_Label => GetString("JointProcess_Length_Label");

        /// <summary>Лейбл параметра "Обороты" (текущие показания)</summary>
        public static string JointProcess_Turns_Label => GetString("JointProcess_Turns_Label");

        /// <summary>Лейбл параметра "Обороты/Мин" (текущие показания)</summary>
        public static string JointProcess_TurnsPerMinute_Label => GetString("JointProcess_TurnsPerMinute_Label");

        /// <summary>Лейбл параметра "Отсчёт, сек" (текущие показания)</summary>
        public static string JointProcess_ElapsedTime_Label => GetString("JointProcess_ElapsedTime_Label");

        /// <summary>Лейбл параметра "Оценка" (результат свинчивания)</summary>
        public static string JointProcess_Evaluation_Label => GetString("JointProcess_Evaluation_Label");

        /// <summary>Лейбл параметра "Итог. момент, Нм" (результат свинчивания)</summary>
        public static string JointProcess_FinalTorque_Label => GetString("JointProcess_FinalTorque_Label");
        /// <summary>Лейбл параметра "Итог. момент буртика, Нм" (результат свинчивания)</summary>
        public static string JointProcess_FinalShoulderTorque_Label => GetString("JointProcess_FinalShoulderTorque_Label");

        /// <summary>Лейбл параметра "Итог. глубина, мм" (результат свинчивания)</summary>
        public static string JointProcess_FinalLength_Label => GetString("JointProcess_FinalLength_Label");

        /// <summary>Лейбл параметра "Итог. обороты" (результат свинчивания)</summary>
        public static string JointProcess_FinalTurns_Label => GetString("JointProcess_FinalTurns_Label");

        /// <summary>Лейбл параметра "Глубина преднав., мм" (результат свинчивания)</summary>
        public static string JointProcess_MVSLength_Label => GetString("JointProcess_MVSLength_Label");

        /// <summary>Лейбл параметра "Глубина силов., мм" (результат свинчивания)</summary>
        public static string JointProcess_MakeupLength_Label => GetString("JointProcess_MakeupLength_Label");

        /// <summary>Лейбл параметра "Опт. момент, Нм" (данные рецепта)</summary>
        public static string JointProcess_OptimalTorque_Label => GetString("JointProcess_OptimalTorque_Label");

        /// <summary>Лейбл параметра "Макс. момент, Нм" (данные рецепта)</summary>
        public static string JointProcess_MaxTorque_Label => GetString("JointProcess_MaxTorque_Label");

        /// <summary>Лейбл параметра "Мин. момент, Нм" (данные рецепта)</summary>
        public static string JointProcess_MinTorque_Label => GetString("JointProcess_MinTorque_Label");

        /// <summary>Лейбл параметра "Момент сброса, Нм" (данные рецепта)</summary>
        public static string JointProcess_DumpTorque_Label => GetString("JointProcess_DumpTorque_Label");

        /// <summary>Лейбл параметра "Макс. глубина, мм" (данные рецепта)</summary>
        public static string JointProcess_MaxLength_Label => GetString("JointProcess_MaxLength_Label");

        /// <summary>Лейбл параметра "Мин. глубина, мм" (данные рецепта)</summary>
        public static string JointProcess_MinLength_Label => GetString("JointProcess_MinLength_Label");

        /// <summary>Лейбл параметра "Сброс глубины, мм" (данные рецепта)</summary>
        public static string JointProcess_DumpLength_Label => GetString("JointProcess_DumpLength_Label");

        /// <summary>Лейбл параметра "Макс. буртик, Нм" (данные рецепта)</summary>
        public static string JointProcess_MaxShoulderTorque_Label => GetString("JointProcess_MaxShoulderTorque_Label");

        /// <summary>Лейбл параметра "Мин. буртик, Нм" (данные рецепта)</summary>
        public static string JointProcess_MinShoulderTorque_Label => GetString("JointProcess_MinShoulderTorque_Label");

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
