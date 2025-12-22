using PNTZ.Mufta.TPCApp.Repository;
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
        public static JointRecipeTable Clone(JointRecipeTable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var copy = new JointRecipeTable
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
        /// <summary>
        /// Копирует данные из source в target
        /// </summary>
        public static void CopyRecipeDataTo(this JointRecipeTable source, JointRecipeTable target)
        {
            // Копируем Id и TimeStamp (важно для новых рецептов)
            target.Id = source.Id;
            target.TimeStamp = source.TimeStamp;

            target.Name = source.Name;
            target.JointMode = source.JointMode;
            target.SelectedThreadType = source.SelectedThreadType;
            target.Thread_step = source.Thread_step;
            target.PLC_PROG_NR = source.PLC_PROG_NR;
            target.HEAD_OPEN_PULSES = source.HEAD_OPEN_PULSES;
            target.TURNS_BREAK = source.TURNS_BREAK;

            // Данные муфты
            target.Box_Len_Min = source.Box_Len_Min;
            target.Box_Len_Max = source.Box_Len_Max;
            target.Box_Moni_Time = source.Box_Moni_Time;

            // Преднавёртка
            target.Pre_Len_Min = source.Pre_Len_Min;
            target.Pre_Len_Max = source.Pre_Len_Max;
            target.Pre_Moni_Time = source.Pre_Moni_Time;

            // Параметры момента
            target.MU_Tq_Dump = source.MU_Tq_Dump;
            target.MU_Tq_Opt = source.MU_Tq_Opt;
            target.MU_Tq_Min = source.MU_Tq_Min;
            target.MU_Tq_Max = source.MU_Tq_Max;
            target.MU_Tq_Ref = source.MU_Tq_Ref;
            target.MU_Tq_Save = source.MU_Tq_Save;
            target.MU_Moni_Time = source.MU_Moni_Time;
            target.MU_TqSpeedRed_1 = source.MU_TqSpeedRed_1;
            target.MU_TqSpeedRed_2 = source.MU_TqSpeedRed_2;

            // Параметры плеча
            target.MU_TqShoulder_Min = source.MU_TqShoulder_Min;
            target.MU_TqShoulder_Max = source.MU_TqShoulder_Max;

            // Параметры длины
            target.MU_Len_Dump = source.MU_Len_Dump;
            target.MU_Len_Speed_1 = source.MU_Len_Speed_1;
            target.MU_Len_Speed_2 = source.MU_Len_Speed_2;
            target.MU_Len_Min = source.MU_Len_Min;
            target.MU_Len_Max = source.MU_Len_Max;
        }

        /// <summary>
        /// Сравнивает два рецепта на равенство всех свойств
        /// </summary>
        /// <param name="recipe1">Первый рецепт</param>
        /// <param name="recipe2">Второй рецепт</param>
        /// <returns>True, если все свойства равны; иначе False</returns>
        public static bool AreEqual(JointRecipeTable recipe1, JointRecipeTable recipe2)
        {
            // Если оба null - равны
            if (recipe1 == null && recipe2 == null)
                return true;

            // Если один null - не равны
            if (recipe1 == null || recipe2 == null)
                return false;

            // Сравниваем свойства рецепта (Id и TimeStamp не сравниваем, т.к. они служебные)
            return recipe1.Name == recipe2.Name &&
                   recipe1.JointMode == recipe2.JointMode &&
                   recipe1.SelectedThreadType == recipe2.SelectedThreadType &&
                   recipe1.Thread_step == recipe2.Thread_step &&
                   recipe1.PLC_PROG_NR == recipe2.PLC_PROG_NR &&
                   recipe1.HEAD_OPEN_PULSES == recipe2.HEAD_OPEN_PULSES &&
                   recipe1.TURNS_BREAK == recipe2.TURNS_BREAK &&

                   // Данные муфты
                   recipe1.Box_Len_Min == recipe2.Box_Len_Min &&
                   recipe1.Box_Len_Max == recipe2.Box_Len_Max &&
                   recipe1.Box_Moni_Time == recipe2.Box_Moni_Time &&

                   // Преднавёртка
                   recipe1.Pre_Len_Min == recipe2.Pre_Len_Min &&
                   recipe1.Pre_Len_Max == recipe2.Pre_Len_Max &&
                   recipe1.Pre_Moni_Time == recipe2.Pre_Moni_Time &&

                   // Параметры момента
                   recipe1.MU_Tq_Dump == recipe2.MU_Tq_Dump &&
                   recipe1.MU_Tq_Opt == recipe2.MU_Tq_Opt &&
                   recipe1.MU_Tq_Min == recipe2.MU_Tq_Min &&
                   recipe1.MU_Tq_Max == recipe2.MU_Tq_Max &&
                   recipe1.MU_Tq_Ref == recipe2.MU_Tq_Ref &&
                   recipe1.MU_Tq_Save == recipe2.MU_Tq_Save &&
                   recipe1.MU_Moni_Time == recipe2.MU_Moni_Time &&
                   recipe1.MU_TqSpeedRed_1 == recipe2.MU_TqSpeedRed_1 &&
                   recipe1.MU_TqSpeedRed_2 == recipe2.MU_TqSpeedRed_2 &&

                   // Параметры плеча
                   recipe1.MU_TqShoulder_Min == recipe2.MU_TqShoulder_Min &&
                   recipe1.MU_TqShoulder_Max == recipe2.MU_TqShoulder_Max &&

                   // Параметры длины
                   recipe1.MU_Len_Dump == recipe2.MU_Len_Dump &&
                   recipe1.MU_Len_Speed_1 == recipe2.MU_Len_Speed_1 &&
                   recipe1.MU_Len_Speed_2 == recipe2.MU_Len_Speed_2 &&
                   recipe1.MU_Len_Min == recipe2.MU_Len_Min &&
                   recipe1.MU_Len_Max == recipe2.MU_Len_Max;
        }
    }
}
