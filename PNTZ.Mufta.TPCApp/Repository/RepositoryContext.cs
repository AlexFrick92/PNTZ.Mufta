using PNTZ.Mufta.TPCApp.Domain;

using Dapper;

using System;
using System.Collections.Generic;

using static PNTZ.Mufta.TPCApp.App;

using Promatis.Core.Logging;

using Microsoft.Data.Sqlite;


namespace PNTZ.Mufta.TPCApp.Repository
{
    public class RepositoryContext
    {

        ILogger logger;

        string StoragePath = App.AppInstance.CurrentDirectory + "/Repository";
        string recipesConnectionString;

        public RepositoryContext()
        {
            recipesConnectionString = $"Data Source={StoragePath}/RecipesData.db;Mode=ReadWriteCreate";

            CreateRecipeTable();

        }

        private void CreateRecipeTable()
        {
            using (var connection = new SqliteConnection(recipesConnectionString))
            {
                connection.Open();
                var createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Recipes

                    (Name TEXT,
                    HEAD_OPEN_PULSES REAL,
                    TURNS_BREAK REAL,
                    PLC_PROG_NR INTEGER,
                    LOG_NO INTEGER,
                    Tq_UNIT INTEGER,
                    SelectedThreadType INTEGER,
                    Thread_step REAL,
                    PIPE_TYPE TEXT,

                    Box_Moni_Time INTEGER,
                    Box_Len_Min REAL,
                    Box_Len_Max REAL,

                    Pre_Moni_Time INTEGER,
                    Pre_Len_Max REAL,
                    Pre_Len_Min REAL,

                    MU_Moni_Time INTERGER,
                    MU_Tq_Ref REAL,
                    MU_Tq_Save REAL,

                    SelectedMode INTEGER,

                    MU_TqSpeedRed_1 REAL,
                    MU_TqSpeedRed_2 REAL,
                    MU_Tq_Dump REAL,
                    MU_Tq_Max REAL,
                    MU_Tq_Min REAL,
                    MU_Tq_Opt REAL,

                    MU_TqShoulder_Min REAL,
                    MU_TqShoulder_Max REAL,

                    MU_Len_Speed_1 REAL,
                    MU_Len_Speed_2 REAL,
                    MU_Len_Dump REAL,
                    MU_Len_Min REAL,
                    MU_Len_Max REAL,

                    MU_JVal_Speed_1 REAL,
                    MU_JVal_Speed_2 REAL,
                    MU_JVal_Dump REAL,
                    MU_JVal_Min REAL,
                    MU_JVal_Max REAL,

                    TimeStamp TEXT
                    )
                ";
                connection.Execute(createTableQuery);
            }
        }
        public void SaveRecipe(JointRecipe rec)
        {

            //if (loadedRecipes.FirstOrDefault(r => r.Name == rec.Name) != null)
              //  UpdateRecipe(rec);
            //else
                InsertRecipe(rec);            

        }
        private void InsertRecipe(JointRecipe rec)
        {
            using (var connection = new SqliteConnection(recipesConnectionString))
            {
                connection.Open();
                var insertQuery = @"
                    INSERT INTO Recipes (

                    Name,
                    HEAD_OPEN_PULSES,
                    TURNS_BREAK,
                    PLC_PROG_NR,
                    LOG_NO,
                    Tq_UNIT,
                    SelectedThreadType,
                    Thread_step,
                    PIPE_TYPE,

                    Box_Moni_Time,
                    Box_Len_Min,
                    Box_Len_Max,

                    Pre_Moni_Time,
                    Pre_Len_Max,
                    Pre_Len_Min,

                    MU_Moni_Time,
                    MU_Tq_Ref,
                    MU_Tq_Save,

                    SelectedMode,

                    MU_TqSpeedRed_1,
                    MU_TqSpeedRed_2,
                    MU_Tq_Dump,
                    MU_Tq_Max,
                    MU_Tq_Min,
                    MU_Tq_Opt,

                    MU_TqShoulder_Min,
                    MU_TqShoulder_Max,

                    MU_Len_Speed_1,
                    MU_Len_Speed_2,
                    MU_Len_Dump,
                    MU_Len_Min,
                    MU_Len_Max,

                    MU_JVal_Speed_1,
                    MU_JVal_Speed_2,
                    MU_JVal_Dump,
                    MU_JVal_Min,
                    MU_JVal_Max,

                    TimeStamp
                    )

                    VALUES (
                    @Name,
                    @HEAD_OPEN_PULSES,
                    @TURNS_BREAK,
                    @PLC_PROG_NR,
                    @LOG_NO,
                    @Tq_UNIT,
                    @SelectedThreadType,
                    @Thread_step,
                    @PIPE_TYPE,

                    @Box_Moni_Time,
                    @Box_Len_Min,
                    @Box_Len_Max,

                    @Pre_Moni_Time,
                    @Pre_Len_Max,
                    @Pre_Len_Min,

                    @MU_Moni_Time,
                    @MU_Tq_Ref,
                    @MU_Tq_Save,

                    @SelectedMode,

                    @MU_TqSpeedRed_1,
                    @MU_TqSpeedRed_2,
                    @MU_Tq_Dump,
                    @MU_Tq_Max,
                    @MU_Tq_Min,
                    @MU_Tq_Opt,

                    @MU_TqShoulder_Min,
                    @MU_TqShoulder_Max,

                    @MU_Len_Speed_1,
                    @MU_Len_Speed_2,
                    @MU_Len_Dump,
                    @MU_Len_Min,
                    @MU_Len_Max,

                    @MU_JVal_Speed_1,
                    @MU_JVal_Speed_2,
                    @MU_JVal_Dump,
                    @MU_JVal_Min,
                    @MU_JVal_Max,

                    @TimeStamp

                    )
                ";

                var recipe = new
                {
                    Name = rec.Name,
                    HEAD_OPEN_PULSES = rec.HEAD_OPEN_PULSES,
                    TURNS_BREAK = rec.TURNS_BREAK,
                    PLC_PROG_NR = rec.PLC_PROG_NR,
                    LOG_NO = rec.LOG_NO,
                    Tq_UNIT = rec.Tq_UNIT,
                    SelectedThreadType = (int)rec.SelectedThreadType,                
                    Thread_step = rec.Thread_step,
                    PIPE_TYPE = rec.PIPE_TYPE,

                    Box_Moni_Time = rec.Box_Moni_Time,
                    Box_Len_Min = rec.Box_Len_Min,
                    Box_Len_Max = rec.Box_Len_Max,

                    Pre_Moni_Time = rec.Pre_Moni_Time,
                    Pre_Len_Max = rec.Pre_Len_Max,
                    Pre_Len_Min = rec.Pre_Len_Min,

                    MU_Moni_Time = rec.MU_Moni_Time,
                    MU_Tq_Ref = rec.MU_Tq_Ref,
                    MU_Tq_Save = rec.MU_Tq_Save,

                    SelectedMode = (int)rec.JointMode,

                    MU_TqSpeedRed_1 = rec.MU_TqSpeedRed_1,
                    MU_TqSpeedRed_2 = rec.MU_TqSpeedRed_2,
                    MU_Tq_Dump = rec.MU_Tq_Dump,
                    MU_Tq_Max = rec.MU_Tq_Max,
                    MU_Tq_Min = rec.MU_Tq_Min,
                    MU_Tq_Opt = rec.MU_Tq_Opt,

                    MU_TqShoulder_Min = rec.MU_TqShoulder_Min,
                    MU_TqShoulder_Max = rec.MU_TqShoulder_Max,

                    MU_Len_Speed_1 = rec.MU_Len_Speed_1,
                    MU_Len_Speed_2 = rec.MU_Len_Speed_2,
                    MU_Len_Dump = rec.MU_Len_Dump,
                    MU_Len_Min = rec.MU_Len_Min,
                    MU_Len_Max = rec.MU_Len_Max,

                    MU_JVal_Speed_1 = rec.MU_Jval_Speed_1,
                    MU_JVal_Speed_2 = rec.MU_Jval_Speed_2,
                    MU_JVal_Dump = rec.MU_Jval_Dump,
                    MU_JVal_Min = rec.MU_Jval_Min,
                    MU_JVal_Max = rec.MU_Jval_Max,

                    TimeStamp = rec.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"),

                };
                connection.Execute(insertQuery, recipe);
            };
        }
        
        public void RemoveRecipe(JointRecipe recipe)
        {

        }

        private void UpdateRecipe(JointRecipe rec)
        {

        }



        public IEnumerable<JointRecipe> LoadRecipes()
        {
            return new List<JointRecipe>();
        }

        public void SaveResult(JointResult result)
        {

        }

        public IEnumerable<JointResult> GetResults()
        {
            return new List<JointResult>();
        }
    }
}
