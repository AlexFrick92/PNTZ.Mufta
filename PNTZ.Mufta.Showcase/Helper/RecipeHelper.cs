using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.Showcase.Helper
{
    public static class RecipeHelper
    {
        /// <summary>
        /// Создать базовый рецепт с заполненными всеми полями
        /// </summary>
        private static JointRecipeTable CreateBaseRecipe(Action<JointRecipeTable> configure)
        {
            var recipe = new JointRecipeTable
            {
                Id = Guid.NewGuid(),
                Name = "TEST_BASE",
                JointMode = JointMode.Length,
                SelectedThreadType = (long)ThreadType.RIGHT,

                // Общие данные
                HEAD_OPEN_PULSES = 100f,
                TURNS_BREAK = 0.5f,
                PLC_PROG_NR = 1,
                LOG_NO = 1,
                Tq_UNIT = 1,
                Thread_step = 25.4f,
                PIPE_TYPE = "TEST_PIPE",

                // Параметры муфты
                Box_Moni_Time = 5000,
                Box_Len_Min = 10f,
                Box_Len_Max = 50f,

                // Параметры преднавёртки
                Pre_Moni_Time = 10000,
                Pre_Len_Max = 100f,
                Pre_Len_Min = 20f,

                // Параметры силового свинчивания общие
                MU_Moni_Time = 15000,
                MU_Tq_Ref = 5000f,
                MU_Tq_Save = 4500f,
                MU_TqSpeedRed_1 = 3000f,
                MU_TqSpeedRed_2 = 4000f,
                MU_Tq_Dump = 2000f,
                MU_Tq_Max = 8000f,
                MU_Tq_Min = 3000f,
                MU_Tq_Opt = 6000f,
                MU_TqShoulder_Min = 2500f,
                MU_TqShoulder_Max = 3500f,

                // Параметры силового свинчивания по длине
                MU_Len_Speed_1 = 50f,
                MU_Len_Speed_2 = 30f,
                MU_Len_Dump = 150f,
                MU_Len_Min = 108f,
                MU_Len_Max = 116f,

                // Параметры силового свинчивания по J
                MU_JVal_Speed_1 = 100f,
                MU_JVal_Speed_2 = 80f,
                MU_JVal_Dump = 500f,
                MU_JVal_Min = 1000f,
                MU_JVal_Max = 2000f,

                TimeStamp = DateTime.Now
            };

            configure?.Invoke(recipe);
            return recipe;
        }

        /// <summary>
        /// Создать тестовый рецепт по длине
        /// </summary>
        public static JointRecipeTable CreateTestRecipeLength()
        {
            return CreateBaseRecipe(recipe =>
            {
                recipe.Name = "TEST_LENGTH";
                recipe.JointMode = JointMode.Length;
                recipe.PLC_PROG_NR = 1;
                recipe.LOG_NO = 1;

                // Средние параметры между Length и Torque              
                recipe.MU_Moni_Time = 16000;

                recipe.MU_Tq_Ref = 6000f;
                recipe.MU_Tq_Save = 5500f;
                recipe.MU_Tq_Dump = 2500f;
                recipe.MU_Tq_Max = 9000f;
                recipe.MU_Tq_Min = 3500f;
                recipe.MU_Tq_Opt = 7000f;

                recipe.MU_Len_Speed_1 = 55f;
                recipe.MU_Len_Speed_2 = 35f;
                recipe.MU_Len_Min = 108f;
                recipe.MU_Len_Max = 112f;
                recipe.MU_Len_Dump = 110f;
            });
        }

        /// <summary>
        /// Создать тестовый рецепт по моменту
        /// </summary>
        public static JointRecipeTable CreateTestRecipeTorque()
        {
            return CreateBaseRecipe(recipe =>
            {
                recipe.Name = "TEST_TORQUE";
                recipe.JointMode = JointMode.Torque;
                recipe.PLC_PROG_NR = 2;
                recipe.LOG_NO = 2;
                recipe.PIPE_TYPE = "TEST_PIPE_TQ";

                recipe.MU_Moni_Time = 18000;
                recipe.MU_Tq_Ref = 7000f;
                recipe.MU_Tq_Save = 6500f;
                recipe.MU_TqSpeedRed_1 = 4000f;
                recipe.MU_TqSpeedRed_2 = 5500f;
                recipe.MU_Tq_Dump = 3000f;
                recipe.MU_Tq_Max = 10000f;
                recipe.MU_Tq_Min = 4000f;
                recipe.MU_Tq_Opt = 8000f;
            });
        }

        /// <summary>
        /// Создать тестовый рецепт по моменту и длине
        /// </summary>
        public static JointRecipeTable CreateTestRecipeTorqueLength()
        {
            return CreateBaseRecipe(recipe =>
            {
                recipe.Name = "TEST_TORQUE_LENGTH";
                recipe.JointMode = JointMode.TorqueLength;
                recipe.PLC_PROG_NR = 3;
                recipe.LOG_NO = 3;
                recipe.PIPE_TYPE = "TEST_PIPE_TQLEN";

                // Средние параметры между Length и Torque              
                recipe.MU_Moni_Time = 16000;
                recipe.MU_Tq_Ref = 6000f;
                recipe.MU_Tq_Save = 5500f;
                recipe.MU_Tq_Dump = 2500f;
                recipe.MU_Tq_Max = 9000f;
                recipe.MU_Tq_Min = 3500f;
                recipe.MU_Tq_Opt = 7000f;

                recipe.MU_Len_Speed_1 = 55f;
                recipe.MU_Len_Speed_2 = 35f;
                recipe.MU_Len_Min = 108f;
                recipe.MU_Len_Max = 112f;
                recipe.MU_Len_Dump = 110f;
            });
        }

        /// <summary>
        /// Создать тестовый рецепт по моменту до упора
        /// </summary>
        public static JointRecipeTable CreateTestRecipeTorqueShoulder()
        {
            return CreateBaseRecipe(recipe =>
            {
                recipe.Name = "TEST_TORQUE_SHOULDER";
                recipe.JointMode = JointMode.TorqueShoulder;
                recipe.PLC_PROG_NR = 3;
                recipe.LOG_NO = 3;
                recipe.PIPE_TYPE = "TEST_PIPE_TQSHL";

                // Средние параметры между Length и Torque              
                recipe.MU_Moni_Time = 16000;
                recipe.MU_Tq_Ref = 6000f;
                recipe.MU_Tq_Save = 5500f;
                recipe.MU_TqSpeedRed_1 = 3500f;
                recipe.MU_TqSpeedRed_2 = 4500f;
                recipe.MU_Tq_Dump = 6700;
                recipe.MU_Tq_Max = 9000f;
                recipe.MU_Tq_Min = 2500f;
                recipe.MU_Tq_Opt = 7000f;
                recipe.MU_TqShoulder_Min = 4000f;
                recipe.MU_TqShoulder_Max = 6200f;
            });
        }
    }
}
