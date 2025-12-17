using System;

namespace PNTZ.Mufta.TPCApp.Domain.Helpers
{
    /// <summary>
    /// Helper-класс для работы с JointRecipe
    /// </summary>
    public static class JointRecipeHelper
    {
        /// <summary>
        /// Создаёт полную копию рецепта
        /// </summary>
        /// <param name="source">Исходный рецепт</param>
        /// <returns>Копия рецепта со всеми свойствами</returns>
        /// <exception cref="ArgumentNullException">Если source равен null</exception>
        public static JointRecipe Clone(JointRecipe source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var copy = new JointRecipe
            {
                Id = source.Id,
                Name = source.Name,
                JointMode = source.JointMode,
                SelectedThreadType = source.SelectedThreadType,
                Thread_step = source.Thread_step,
                PLC_PROG_NR = source.PLC_PROG_NR,
                HEAD_OPEN_PULSES = source.HEAD_OPEN_PULSES,
                TURNS_BREAK = source.TURNS_BREAK,

                // Данные муфты
                Box_Len_Min = source.Box_Len_Min,
                Box_Len_Max = source.Box_Len_Max,
                Box_Moni_Time = source.Box_Moni_Time,

                // Преднавёртка
                Pre_Len_Min = source.Pre_Len_Min,
                Pre_Len_Max = source.Pre_Len_Max,
                Pre_Moni_Time = source.Pre_Moni_Time,

                // Параметры момента
                MU_Tq_Dump = source.MU_Tq_Dump,
                MU_Tq_Opt = source.MU_Tq_Opt,
                MU_Tq_Min = source.MU_Tq_Min,
                MU_Tq_Max = source.MU_Tq_Max,
                MU_Tq_Ref = source.MU_Tq_Ref,
                MU_Tq_Save = source.MU_Tq_Save,
                MU_Moni_Time = source.MU_Moni_Time,
                MU_TqSpeedRed_1 = source.MU_TqSpeedRed_1,
                MU_TqSpeedRed_2 = source.MU_TqSpeedRed_2,

                // Параметры плеча
                MU_TqShoulder_Min = source.MU_TqShoulder_Min,
                MU_TqShoulder_Max = source.MU_TqShoulder_Max,

                // Параметры длины
                MU_Len_Dump = source.MU_Len_Dump,
                MU_Len_Speed_1 = source.MU_Len_Speed_1,
                MU_Len_Speed_2 = source.MU_Len_Speed_2,
                MU_Len_Min = source.MU_Len_Min,
                MU_Len_Max = source.MU_Len_Max
            };

            return copy;
        }
    }
}
