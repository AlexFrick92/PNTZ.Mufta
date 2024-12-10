using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public static class JointRecipeMapperHelper
    {
        public static JointRecipeMapper FromJointRecipe(this JointRecipeMapper mapper, JointRecipe recipe)
        {
            return new JointRecipeMapper()
            {
                Name = recipe.Name,
                HEAD_OPEN_PULSES = recipe.HEAD_OPEN_PULSES,
                TURNS_BREAK = recipe.TURNS_BREAK,
                PLC_PROG_NR = recipe.PLC_PROG_NR,
                LOG_NO = recipe.LOG_NO,
                Tq_UNIT = recipe.Tq_UNIT,
                SelectedThreadType = (int)recipe.SelectedThreadType,                
                Thread_step = recipe.Thread_step,
                PIPE_TYPE = recipe.PIPE_TYPE,

                Box_Moni_Time = recipe.Box_Moni_Time,
                Box_Len_Min = recipe.Box_Len_Min,
                Box_Len_Max = recipe.Box_Len_Max,

                Pre_Moni_Time = recipe.Pre_Moni_Time,
                Pre_Len_Max = recipe.Pre_Len_Max,
                Pre_Len_Min = recipe.Pre_Len_Min,

                MU_Moni_Time = recipe.MU_Moni_Time,
                MU_Tq_Ref = recipe.MU_Tq_Ref,
                MU_Tq_Save = recipe.MU_Tq_Save,

                SelectedMode = (int)recipe.JointMode,

                MU_TqSpeedRed_1 = recipe.MU_TqSpeedRed_1,
                MU_TqSpeedRed_2 = recipe.MU_TqSpeedRed_2,
                MU_Tq_Dump = recipe.MU_Tq_Dump,
                MU_Tq_Max = recipe.MU_Tq_Max,
                MU_Tq_Min = recipe.MU_Tq_Min,
                MU_Tq_Opt = recipe.MU_Tq_Opt,

                MU_TqShoulder_Min = recipe.MU_TqShoulder_Min,
                MU_TqShoulder_Max = recipe.MU_TqShoulder_Max,

                MU_Len_Speed_1 = recipe.MU_Len_Speed_1,
                MU_Len_Speed_2 = recipe.MU_Len_Speed_2,
                MU_Len_Dump = recipe.MU_Len_Dump,
                MU_Len_Min = recipe.MU_Len_Min,
                MU_Len_Max = recipe.MU_Len_Max,

                MU_JVal_Speed_1 = recipe.MU_JVal_Speed_1,
                MU_JVal_Speed_2 = recipe.MU_JVal_Speed_2,
                MU_JVal_Dump = recipe.MU_JVal_Dump,
                MU_JVal_Min = recipe.MU_JVal_Min,
                MU_JVal_Max = recipe.MU_JVal_Max,

                TimeStamp = recipe.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"),
            };            
        }

        public static JointRecipe ToJointRecipe(this JointRecipeMapper mapper)
        {
            return new JointRecipe()
            {
                Name = mapper.Name,
                HEAD_OPEN_PULSES = (float)mapper.HEAD_OPEN_PULSES,
                TURNS_BREAK = (float)mapper.TURNS_BREAK,
                PLC_PROG_NR = (ushort)mapper.PLC_PROG_NR,
                LOG_NO = (ushort)mapper.LOG_NO,
                Tq_UNIT = (ushort)mapper.Tq_UNIT,
                SelectedThreadType = (ThreadType)mapper.SelectedThreadType,
                Thread_step = (float)mapper.Thread_step,
                PIPE_TYPE = mapper.PIPE_TYPE,

                Box_Moni_Time = (int)mapper.Box_Moni_Time,
                Box_Len_Min = (float)mapper.Box_Len_Min,
                Box_Len_Max = (float)mapper.Box_Len_Max,

                Pre_Moni_Time = (int)mapper.Pre_Moni_Time,
                Pre_Len_Max = (float)mapper.Pre_Len_Max,
                Pre_Len_Min = (float)mapper.Pre_Len_Min,

                MU_Moni_Time = (int)mapper.MU_Moni_Time,
                MU_Tq_Ref = (float)mapper.MU_Tq_Ref,
                MU_Tq_Save = (float)mapper.MU_Tq_Save,

                JointMode = (JointMode)mapper.SelectedMode,

                MU_TqSpeedRed_1 = (float)mapper.MU_TqSpeedRed_1,
                MU_TqSpeedRed_2 = (float)mapper.MU_TqSpeedRed_2,
                MU_Tq_Dump = (float)mapper.MU_Tq_Dump,
                MU_Tq_Max = (float)mapper.MU_Tq_Max,
                MU_Tq_Min = (float)mapper.MU_Tq_Min,
                MU_Tq_Opt = (float)mapper.MU_Tq_Opt,

                MU_TqShoulder_Min = (float)mapper.MU_TqShoulder_Min,
                MU_TqShoulder_Max = (float)mapper.MU_TqShoulder_Max,

                MU_Len_Speed_1 = (float)mapper.MU_Len_Speed_1,
                MU_Len_Speed_2 = (float)mapper.MU_Len_Speed_2,
                MU_Len_Dump = (float)mapper.MU_Len_Dump,
                MU_Len_Min = (float)mapper.MU_Len_Min,
                MU_Len_Max = (float)mapper.MU_Len_Max,

                MU_JVal_Speed_1 = (float)mapper.MU_JVal_Speed_1,
                MU_JVal_Speed_2 = (float)mapper.MU_JVal_Speed_2,
                MU_JVal_Dump = (float)mapper.MU_JVal_Dump,
                MU_JVal_Min = (float)mapper.MU_JVal_Min,
                MU_JVal_Max = (float)mapper.MU_JVal_Max,

                TimeStamp = DateTime.Parse(mapper.TimeStamp)
            };
        }
    }
}
