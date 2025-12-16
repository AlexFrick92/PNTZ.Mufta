using System;

using static PNTZ.Mufta.TPCApp.App;


namespace PNTZ.Mufta.TPCApp.Domain
{

    public class JointRecipe
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        // Общие данные
        public float HEAD_OPEN_PULSES { get; set; }
        public float TURNS_BREAK { get; set; }
        public ushort PLC_PROG_NR { get; set; }
        public ushort LOG_NO { get; set; }
        public ushort Tq_UNIT { get; set; }
        public ushort Thread_type
        {
            get
            {
                switch(SelectedThreadType)
                {
                    case ThreadType.RIGHT:
                        return 1;
                    case ThreadType.LEFT:
                        return 2;
                    default:
                        return 1;
                }    
            }
        }


        public ThreadType SelectedThreadType { get; set; }
        public float Thread_step { get; set; }
        public string PIPE_TYPE { get; set; }

        // Параметры муфты
        public int Box_Moni_Time { get; set; }
        public float Box_Len_Min { get; set; }
        public float Box_Len_Max { get; set; }

        //Параметры преднавёртки
        public int Pre_Moni_Time { get; set; }
        public float Pre_Len_Max { get; set; }
        public float Pre_Len_Min { get; set; }

        //Параметры силового свинчивания общие
        public int MU_Moni_Time { get; set; }
        public float MU_Tq_Ref { get; set; }
        public float MU_Tq_Save { get; set; }       
        public ushort MU_Makeup_Mode // 0 - по моменту, 1 - по длине, 2 - по JVal
        {
            get
            {
                switch(JointMode)
                {
                    case JointMode.Torque:
                    case JointMode.TorqueShoulder:
                        return 0;

                    case JointMode.Length:
                    case JointMode.TorqueLength:
                        return 1;

                    case JointMode.Jval:
                    case JointMode.TorqueJVal:
                        return 2;

                    default:
                        return 0;
                }    
            }
        }
        public JointMode JointMode { get; set; }
        public float MU_TqSpeedRed_1 { get; set; }
        public float MU_TqSpeedRed_2 { get; set; }
        public float MU_Tq_Dump { get; set; }
        public float MU_Tq_Max { get; set; }
        public float MU_Tq_Min { get; set; }
        public float MU_Tq_Opt { get; set; }
        public float MU_TqShoulder_Min { get; set; }
        public float MU_TqShoulder_Max { get; set; }

        //Параметры силового свинчивания по длине
        public float MU_Len_Speed_1 { get; set; }
        public float MU_Len_Speed_2 { get; set; }
        public float MU_Len_Dump { get; set; }
        public float MU_Len_Min { get; set; }
        public float MU_Len_Max { get; set; }

        //Параметры силового свинчивания по J
        public float MU_JVal_Speed_1 { get; set; }
        public float MU_JVal_Speed_2 { get; set; }
        public float MU_JVal_Dump { get; set; }
        public float MU_JVal_Min { get; set; }
        public float MU_JVal_Max { get; set; }
        

        public DateTime TimeStamp { get; set; }
    }    
}
