using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public class JointRecipeMapper
    {
        public string Name { get; set; }
        public double HEAD_OPEN_PULSES { get; set; }
        public double TURNS_BREAK { get; set; }
        public long PLC_PROG_NR { get; set; }
        public long LOG_NO { get; set; }
        public long Tq_UNIT { get; set; }
        public long SelectedThreadType { get; set; }
        public double Thread_step { get; set; }
        public string PIPE_TYPE { get; set; }

        public long Box_Moni_Time { get; set; }
        public double Box_Len_Min { get; set; }
        public double Box_Len_Max { get; set; }

        public long Pre_Moni_Time { get; set; }
        public double Pre_Len_Max { get; set; }
        public double Pre_Len_Min { get; set; }

        public long MU_Moni_Time { get; set; }
        public double MU_Tq_Ref { get; set; }
        public double MU_Tq_Save { get; set; }

        public long SelectedMode { get; set; }


        public double MU_TqSpeedRed_1 { get; set; }
        public double MU_TqSpeedRed_2 { get; set; }
        public double MU_Tq_Dump { get; set; }
        public double MU_Tq_Max { get; set; }
        public double MU_Tq_Min { get; set; }
        public double MU_Tq_Opt { get; set; }

        public double MU_TqShoulder_Min { get; set; }
        public double MU_TqShoulder_Max { get; set; }

        public double MU_Len_Speed_1 { get; set; }
        public double MU_Len_Speed_2 { get; set; }
        public double MU_Len_Dump { get; set; }
        public double MU_Len_Min { get; set; }
        public double MU_Len_Max { get; set; }

        public double MU_JVal_Speed_1 { get; set; }
        public double MU_JVal_Speed_2 { get; set; }
        public double MU_JVal_Dump { get; set; }
        public double MU_JVal_Min { get; set; }
        public double MU_JVal_Max { get; set; }

        public string TimeStamp { get; set; }
    }
}
