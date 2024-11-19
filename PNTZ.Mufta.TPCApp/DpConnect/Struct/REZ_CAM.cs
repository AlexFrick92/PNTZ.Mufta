using PNTZ.Mufta.TPCApp.Domain;
using System;


namespace PNTZ.Mufta.TPCApp.DpConnect.Struct
{
    public class REZ_CAM
    {

        public float MU_Tq_Max { get; set; }
        public float MU_Tq_Min { get; set; }
        public float MU_Tq_Dump { get; set; }
        public float MU_TqSpeed_Red_1 { get; set; }
        public float MU_TqSpeed_Red_2 { get; set; }
        public float MU_Tq_Ref { get; set; }
        public float MU_Tq_Save { get; set; }


        public float MU_JVal_Max { get; set; }
        public float MU_JVal_Min { get; set; }
        public float MU_JVAL_Dump { get; set; }        
        public float MU_JVal_Speed_1 { get; set; }
        public float MU_JVAL_Speed_2 { get; set; }


        public float Mu_Len_Max { get; set; }
        public float Mu_Len_Min { get; set; }
        public float MU_Len_Dump { get; set; }
        public float MU_Len_Speed_1 { get; set; }
        public float MU_Len_Speed_2 { get; set; }


        public ushort MU_Makeup_Mode { get; set; }
        public int MU_Moni_Time { get; set; }

    }

    static public class REZ_CAM_Helper
    {
        public static REZ_CAM FromRecipe(this REZ_CAM rez, JointRecipe recipe)
        {
            return new REZ_CAM();
        }
    }
}
