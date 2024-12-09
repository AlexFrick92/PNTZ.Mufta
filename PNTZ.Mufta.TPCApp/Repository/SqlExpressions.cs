using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Repository
{
    static internal class SqlExpressions
    {
        public static readonly string CreateTable =
            "CREATE TABLE IF NOT EXISTS Recipes" +

            "(Name TEXT," +
            "HEAD_OPEN_PULSES REAL," +
            "TURNS_BREAK REAL," +
            "PLC_PROG_NR INTEGER," +
            "LOG_NO INTEGER," +
            "Tq_UNIT INTEGER," +
            "SelectedThreadType INTEGER," +
            "Thread_step REAL," +
            "PIPE_TYPE TEXT," +

            "Box_Moni_Time INTEGER," +
            "Box_Len_Min REAL," +
            "Box_Len_Max REAL," +

            "Pre_Moni_Time INTEGER," +
            "Pre_Len_Max REAL," +
            "Pre_Len_Min REAL," +

            "MU_Moni_Time INTERGER," +
            "MU_Tq_Ref REAL," +
            "MU_Tq_Save REAL," +

            "SelectedMode INTEGER," +

            "MU_TqSpeedRed_1 REAL," +
            "MU_TqSpeedRed_2 REAL," +
            "MU_Tq_Dump REAL," +
            "MU_Tq_Max REAL," +
            "MU_Tq_Min REAL," +
            "MU_Tq_Opt REAL," +

            "MU_TqShoulder_Min REAL," +
            "MU_TqShoulder_Max REAL," +

            "MU_Len_Speed_1 REAL," +
            "MU_Len_Speed_2 REAL," +
            "MU_Len_Dump REAL," +
            "MU_Len_Min REAL," +
            "MU_Len_Max REAL," +

            "MU_JVal_Speed_1 REAL," +
            "MU_JVal_Speed_2 REAL," +
            "MU_JVal_Dump REAL," +
            "MU_JVal_Min REAL," +
            "MU_JVal_Max REAL," +

            "TimeStamp TEXT" + 
            ")";

        public static readonly string InsertRecipe =
            "INSERT INTO Recipes " +

            "(Name," +
            "HEAD_OPEN_PULSES," +
            "TURNS_BREAK," +
            "PLC_PROG_NR," +
            "LOG_NO," +
            "Tq_UNIT," +
            "SelectedThreadType," +
            "Thread_step," +
            "PIPE_TYPE," +

            "Box_Moni_Time," +
            "Box_Len_Min," +
            "Box_Len_Max," +

            "Pre_Moni_Time," +
            "Pre_Len_Max," +
            "Pre_Len_Min," +

            "MU_Moni_Time," +
            "MU_Tq_Ref," +
            "MU_Tq_Save," +

            "SelectedMode," +

            "MU_TqSpeedRed_1," +
            "MU_TqSpeedRed_2," +
            "MU_Tq_Dump," +
            "MU_Tq_Max," +
            "MU_Tq_Min," +
            "MU_Tq_Opt," +

            "MU_TqShoulder_Min," +
            "MU_TqShoulder_Max," +

            "MU_Len_Speed_1," +
            "MU_Len_Speed_2," +
            "MU_Len_Dump," +
            "MU_Len_Min," +
            "MU_Len_Max," +

            "MU_JVal_Speed_1," +
            "MU_JVal_Speed_2," +
            "MU_JVal_Dump," +
            "MU_JVal_Min," +
            "MU_JVal_Max," +

            "TimeStamp" +
            
            ")" +
            "VALUES ";

        public static readonly string SelectRecipes =
            "SELECT * FROM Recipes ORDER BY TimeStamp DESC;";
    }
}
