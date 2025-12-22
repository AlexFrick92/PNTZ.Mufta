using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
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
        public static REZ_Muffe FromRecipe(this REZ_Muffe instance, JointRecipeTable recipe)
        {
            REZ_Muffe rez = new REZ_Muffe();

            rez.Box_Len_Max = recipe.Box_Len_Max / 1000;
            rez.Box_Len_Min = recipe.Box_Len_Min / 1000;
            rez.Box_Moni_Time = recipe.Box_Moni_Time;

            return rez;
        }
    }
}
