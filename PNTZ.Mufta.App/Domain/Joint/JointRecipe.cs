
namespace PNTZ.Mufta.App.Domain.Joint
{
    public class JointRecipe
    {
        public float HEAD_OPEN_PULSES { get; set; }
        public float TURNS_BREAK { get; set; }
        public ushort PLC_PROG_NR { get; set; }
        public ushort LOG_NO { get; set; }
        public ushort Tq_UNIT { get; set; }
        public ushort Thread_type { get; set; }
        public float Thread_step { get; set; }
        public string PIPE_TYPE { get; set; }
        public int Box_Moni_Time { get; set; }
        public float Box_Len_Min { get; set; }
        public float Box_Len_Max { get; set; }
        public int Pre_Moni_Time { get; set; }        
        public float Pre_Len_Min { get; set; }
        public float Pre_Len_Max { get; set; }        
        public int MU_Monit_Time { get; set; }
        public float MU_Tq_Ref { get; set; }
        public ushort MU_Makeup_Mode { get; set; }
        public float MU_TqSpeedRed_1 { get; set; }
        public float MU_TqSpeedRed_2 { get; set; }
        public float MU_Tq_Dump { get; set; }
        public float MU_Tq_Max { get; set; }
        public float MU_Tq_Min { get; set; }
        public float MU_Len_Speed_1 { get; set; }
        public float MU_Len_Speed_2 { get; set; }
        public float MU_Len_Dump { get; set; }
        public float Mu_Len_Min { get; set; }
        public float Mu_Len_Max { get; set; }
        public float MU_Jval_Speed_1 { get; set; }
        public float MU_Jval_Speed_2 { get; set; }
        public float MU_Jval_Dump { get; set; }
        public float MU_Jval_Min { get; set; }
        public float MU_Jval_Max { get; set; }
        public float MU_Tq_Save { get; set; }       
    }
}
