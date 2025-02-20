using LinqToDB.Mapping;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Repository
{
    [Table(Name = "Recipes")]
    public class JointRecipeTable
    {

        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column]
        public string Name { get; set; }
        [Column]
        public double HEAD_OPEN_PULSES { get; set; }
        [Column]
        public double TURNS_BREAK { get; set; }
        [Column]
        public long PLC_PROG_NR { get; set; }
        [Column]
        public long LOG_NO { get; set; }
        [Column]
        public long Tq_UNIT { get; set; }
        [Column]
        public long SelectedThreadType { get; set; }
        [Column]
        public double Thread_step { get; set; }
        [Column]
        public string PIPE_TYPE { get; set; }

        [Column]
        public long Box_Moni_Time { get; set; }
        [Column]
        public double Box_Len_Min { get; set; }
        [Column]
        public double Box_Len_Max { get; set; }


        [Column]
        public long Pre_Moni_Time { get; set; }
        [Column]
        public double Pre_Len_Max { get; set; }
        [Column]
        public double Pre_Len_Min { get; set; }

        [Column]
        public long MU_Moni_Time { get; set; }
        [Column]
        public double MU_Tq_Ref { get; set; }
        [Column]
        public double MU_Tq_Save { get; set; }

        [Column]
        public long SelectedMode { get; set; }

        [Column]
        public double MU_TqSpeedRed_1 { get; set; }
        [Column]
        public double MU_TqSpeedRed_2 { get; set; }
        [Column]
        public double MU_Tq_Dump { get; set; }
        [Column]
        public double MU_Tq_Max { get; set; }
        [Column]
        public double MU_Tq_Min { get; set; }
        [Column]
        public double MU_Tq_Opt { get; set; }

        [Column]
        public double MU_TqShoulder_Min { get; set; }
        [Column]
        public double MU_TqShoulder_Max { get; set; }

        [Column]
        public double MU_Len_Speed_1 { get; set; }
        [Column]
        public double MU_Len_Speed_2 { get; set; }
        [Column]
        public double MU_Len_Dump { get; set; }
        [Column]
        public double MU_Len_Min { get; set; }
        [Column]
        public double MU_Len_Max { get; set; }

        [Column]
        public double MU_JVal_Speed_1 { get; set; }
        [Column]
        public double MU_JVal_Speed_2 { get; set; }
        [Column]
        public double MU_JVal_Dump { get; set; }
        [Column]
        public double MU_JVal_Min { get; set; }

        [Column]
        public double MU_JVal_Max { get; set; }

        [Column]
        public string TimeStamp { get; set; }


        public JointRecipeTable FromJointRecipe(JointRecipe recipe)
        {
            if (recipe == null) throw new ArgumentNullException("Recipe is Null");

            Name = recipe.Name;
            HEAD_OPEN_PULSES = recipe.HEAD_OPEN_PULSES;
            TURNS_BREAK = recipe.TURNS_BREAK;
            PLC_PROG_NR = recipe.PLC_PROG_NR;
            LOG_NO = recipe.LOG_NO;
            Tq_UNIT = recipe.Tq_UNIT;
            SelectedThreadType = (int)recipe.SelectedThreadType;
            Thread_step = recipe.Thread_step;
            PIPE_TYPE = recipe.PIPE_TYPE;

            Box_Moni_Time = recipe.Box_Moni_Time;
            Box_Len_Min = recipe.Box_Len_Min;
            Box_Len_Max = recipe.Box_Len_Max;

            Pre_Moni_Time = recipe.Pre_Moni_Time;
            Pre_Len_Max = recipe.Pre_Len_Max;
            Pre_Len_Min = recipe.Pre_Len_Min;

            MU_Moni_Time = recipe.MU_Moni_Time;
            MU_Tq_Ref = recipe.MU_Tq_Ref;
            MU_Tq_Save = recipe.MU_Tq_Save;

            SelectedMode = (int)recipe.JointMode;

            MU_TqSpeedRed_1 = recipe.MU_TqSpeedRed_1;
            MU_TqSpeedRed_2 = recipe.MU_TqSpeedRed_2;
            MU_Tq_Dump = recipe.MU_Tq_Dump;
            MU_Tq_Max = recipe.MU_Tq_Max;
            MU_Tq_Min = recipe.MU_Tq_Min;
            MU_Tq_Opt = recipe.MU_Tq_Opt;

            MU_TqShoulder_Min = recipe.MU_TqShoulder_Min;
            MU_TqShoulder_Max = recipe.MU_TqShoulder_Max;

            MU_Len_Speed_1 = recipe.MU_Len_Speed_1;
            MU_Len_Speed_2 = recipe.MU_Len_Speed_2;
            MU_Len_Dump = recipe.MU_Len_Dump;
            MU_Len_Min = recipe.MU_Len_Min;
            MU_Len_Max = recipe.MU_Len_Max;

            MU_JVal_Speed_1 = recipe.MU_JVal_Speed_1;
            MU_JVal_Speed_2 = recipe.MU_JVal_Speed_2;
            MU_JVal_Dump = recipe.MU_JVal_Dump;
            MU_JVal_Min = recipe.MU_JVal_Min;
            MU_JVal_Max = recipe.MU_JVal_Max;

            TimeStamp = recipe.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");

            return this;
            

        }

        public JointRecipe ToJointRecipe()
        {
            return new JointRecipe()
            {
                Name = this.Name,
                HEAD_OPEN_PULSES = (float)this.HEAD_OPEN_PULSES,
                TURNS_BREAK = (float)this.TURNS_BREAK,
                PLC_PROG_NR = (ushort)this.PLC_PROG_NR,
                LOG_NO = (ushort)this.LOG_NO,
                Tq_UNIT = (ushort)this.Tq_UNIT,
                SelectedThreadType = (ThreadType)this.SelectedThreadType,
                Thread_step = (float)this.Thread_step,
                PIPE_TYPE = this.PIPE_TYPE,

                Box_Moni_Time = (int)this.Box_Moni_Time,
                Box_Len_Min = (float)this.Box_Len_Min,
                Box_Len_Max = (float)this.Box_Len_Max,

                Pre_Moni_Time = (int)this.Pre_Moni_Time,
                Pre_Len_Max = (float)this.Pre_Len_Max,
                Pre_Len_Min = (float)this.Pre_Len_Min,

                MU_Moni_Time = (int)this.MU_Moni_Time,
                MU_Tq_Ref = (float)this.MU_Tq_Ref,
                MU_Tq_Save = (float)this.MU_Tq_Save,

                JointMode = (JointMode)this.SelectedMode,

                MU_TqSpeedRed_1 = (float)this.MU_TqSpeedRed_1,
                MU_TqSpeedRed_2 = (float)this.MU_TqSpeedRed_2,
                MU_Tq_Dump = (float)this.MU_Tq_Dump,
                MU_Tq_Max = (float)this.MU_Tq_Max,
                MU_Tq_Min = (float)this.MU_Tq_Min,
                MU_Tq_Opt = (float)this.MU_Tq_Opt,

                MU_TqShoulder_Min = (float)this.MU_TqShoulder_Min,
                MU_TqShoulder_Max = (float)this.MU_TqShoulder_Max,

                MU_Len_Speed_1 = (float)this.MU_Len_Speed_1,
                MU_Len_Speed_2 = (float)this.MU_Len_Speed_2,
                MU_Len_Dump = (float)this.MU_Len_Dump,
                MU_Len_Min = (float)this.MU_Len_Min,
                MU_Len_Max = (float)this.MU_Len_Max,

                MU_JVal_Speed_1 = (float)this.MU_JVal_Speed_1,
                MU_JVal_Speed_2 = (float)this.MU_JVal_Speed_2,
                MU_JVal_Dump = (float)this.MU_JVal_Dump,
                MU_JVal_Min = (float)this.MU_JVal_Min,
                MU_JVal_Max = (float)this.MU_JVal_Max,

                TimeStamp = DateTime.Parse(this.TimeStamp)
            };
        }
    }
}
