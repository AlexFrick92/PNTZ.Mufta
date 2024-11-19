using PNTZ.Mufta.TPCApp.Domain;
using System;

namespace PNTZ.Mufta.TPCApp.DpConnect.Struct
{
    public class REZ_Muffe
    {
        public float Box_Len_Max { get; set; }
        public float Box_Len_Min { get; set; }
        public int  Box_Moni_Time { get; set; }
    }

    static public class REZ_Muffe_Helper
    {
        public static REZ_Muffe FromRecipe(this REZ_Muffe rez, JointRecipe recipe)
        {
            return new REZ_Muffe();
        }
    }
}
