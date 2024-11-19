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
        public static REZ_CAM FromRecipe(this REZ_CAM instance, JointRecipe recipe)
        {
            REZ_CAM rez = new REZ_CAM();

            rez.MU_Tq_Max = recipe.MU_Tq_Max;
            rez.MU_Tq_Min = recipe.MU_Tq_Min;
            rez.MU_Tq_Dump = recipe.MU_Tq_Dump;
            rez.MU_TqSpeed_Red_1 = recipe.MU_TqSpeedRed_1;
            rez.MU_TqSpeed_Red_2 = recipe.MU_TqSpeedRed_2;
            rez.MU_Tq_Ref = recipe.MU_Tq_Ref;
            rez.MU_Tq_Save = recipe.MU_Tq_Save;

            rez.MU_JVal_Max = recipe.MU_Jval_Max;
            rez.MU_JVal_Min = recipe.MU_Jval_Min;
            rez.MU_JVAL_Dump = recipe.MU_Jval_Dump;
            rez.MU_JVal_Speed_1 = recipe.MU_Jval_Speed_1;
            rez.MU_JVAL_Speed_2 = recipe.MU_Jval_Speed_2;

            rez.Mu_Len_Max = recipe.MU_Len_Max;
            rez.Mu_Len_Min = recipe.MU_Len_Min;
            rez.MU_Len_Dump = recipe.MU_Len_Dump;
            rez.MU_Len_Speed_1 = recipe.MU_Len_Speed_1;
            rez.MU_Len_Speed_2 = recipe.MU_Len_Speed_2;

            rez.MU_Makeup_Mode = recipe.MU_Makeup_Mode;
            rez.MU_Moni_Time = recipe.MU_Moni_Time;

            return rez;
        }
    }
}
