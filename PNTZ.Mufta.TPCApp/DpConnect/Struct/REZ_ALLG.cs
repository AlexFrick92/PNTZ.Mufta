using PNTZ.Mufta.TPCApp.Domain;

using System;

namespace PNTZ.Mufta.TPCApp.DpConnect.Struct
{
    public class REZ_ALLG
    {
        public float HEAD_OPEN_PULSES { get; set; }

        public ushort LOG_NO { get; set; }

        public string PIPE_TYPE { get; set; }

        public ushort PLC_PROG_NR { get; set; }

        public float TURNS_BREAK { get; set; }

        public ushort Thread_type { get; set; }

        public ushort Tq_UNIT { get; set; }
        
    }

    static public class REZ_ALLG_Helper
    {
        public static REZ_ALLG FromRecipe(this REZ_ALLG instance, JointRecipe recipe)
        {
            REZ_ALLG rez = new REZ_ALLG();

            rez.HEAD_OPEN_PULSES = recipe.HEAD_OPEN_PULSES;
            rez.LOG_NO = recipe.LOG_NO;
            rez.PIPE_TYPE = recipe.PIPE_TYPE;
            rez.PLC_PROG_NR = recipe.PLC_PROG_NR;
            rez.TURNS_BREAK = recipe.TURNS_BREAK;
            rez.Thread_type = recipe.Thread_type;
            rez.Tq_UNIT = recipe.Tq_UNIT;

            return rez;
        }
    }

}
