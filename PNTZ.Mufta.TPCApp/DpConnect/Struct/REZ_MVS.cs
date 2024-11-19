using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.DpConnect.Struct
{
    public class REZ_MVS
    {
        public float Pre_Len_Max { get; set; }
        public float Pre_Len_Min { get; set; }
        public int Pre_Moni_Time { get; set; }
    }
    static public class REZ_MVS_Helper
    {
        public static REZ_MVS FromRecipe(this REZ_MVS rez, JointRecipe recipe)
        {
            return new REZ_MVS();
        }
    }
}
