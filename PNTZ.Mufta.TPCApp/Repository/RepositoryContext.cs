using PNTZ.Mufta.TPCApp.Domain;

using Dapper;

using System;
using System.Collections.Generic;

using static PNTZ.Mufta.TPCApp.App;

using Promatis.Core.Logging;

using Microsoft.Data.Sqlite;

using System.Linq;
using System.CodeDom;


namespace PNTZ.Mufta.TPCApp.Repository
{
    public class RepositoryContext
    {

        ILogger logger;

        string StoragePath = App.AppInstance.CurrentDirectory + "/Repository";
        readonly string recipeTableName = "Recipes";
        string recipesConnectionString;

        List<JointRecipe> loadedRecipes = new List<JointRecipe> ();

        public RepositoryContext()
        {
            recipesConnectionString = $"Data Source={StoragePath}/RecipesData.db;Mode=ReadWriteCreate";

            CreateTable(recipeTableName, recipesConnectionString);

            LoadRecipes();

        }
        private void CreateTable(string tableName, string connectionString)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var createTableQuery = GenerateCreateTableSql<JointRecipeMapper>(tableName);
                connection.Execute(createTableQuery);
            }
        }
        private string GenerateCreateTableSql<T>(string TableName)
        {           
            var properties = typeof(T).GetProperties(); 

            var columns = properties.Select(p =>
            {
                // Определение типа для каждого свойства
                var columnType = GetSQLiteColumnType(p.PropertyType);

                return $"{p.Name} {columnType}";
            });

            var sql = $"CREATE TABLE IF NOT EXISTS {TableName} ({string.Join(", ", columns)});";

            return sql;
        }
        private static string GetSQLiteColumnType(Type propertyType)
        {
            // Маппинг типов .NET в типы SQLite
            if (propertyType == typeof(int) || propertyType == typeof(long) || propertyType == typeof(short))
                return "INTEGER";
            if (propertyType == typeof(bool))
                return "INTEGER"; // SQLite не имеет отдельного типа для boolean, используем INTEGER
            if (propertyType == typeof(string))
                return "TEXT";
            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTimeOffset))
                return "TEXT"; // В SQLite DateTime обычно сохраняется в текстовом формате (ISO 8601)
            if (propertyType == typeof(float) || propertyType == typeof(double) || propertyType == typeof(decimal))
                return "REAL";
            if (propertyType == typeof(byte[]) || propertyType == typeof(Guid))
                return "BLOB";

            return "TEXT"; // По умолчанию используем TEXT
        }


        //Операции над рецептами

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

                JointRecipeMapper recipe = new JointRecipeMapper()
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

                    MU_JVal_Speed_1 = rec.MU_JVal_Speed_1,
                    MU_JVal_Speed_2 = rec.MU_JVal_Speed_2,
                    MU_JVal_Dump = rec.MU_JVal_Dump,
                    MU_JVal_Min = rec.MU_JVal_Min,
                    MU_JVal_Max = rec.MU_JVal_Max,

                    TimeStamp = rec.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"),

                };
                connection.Execute(insertQuery, recipe);
            };
        }        


        private void UpdateRecipe(JointRecipe rec)
        {
            using (var connection = new SqliteConnection(recipesConnectionString))
            {
                connection.Open();
                var updateQuery = @"
                    UPDATE Users SET
                    Name = @Name


                ";
                var recipe = new JointRecipeMapper();

                connection.Execute(updateQuery, recipe);
            };
        }
        
        public void RemoveRecipe(JointRecipe recipe)
        {

        }

        public IEnumerable<JointRecipe> LoadRecipes()
        {
            
            using (var connection = new SqliteConnection(recipesConnectionString))
            {
                connection.Open();
                var selectQuery = $"SELECT * FROM {recipeTableName}";
                var recipesFromDb = connection.Query(selectQuery).ToList();

                loadedRecipes.Clear();

                foreach (var rec in recipesFromDb)
                {
                    loadedRecipes.Add(
                        new JointRecipe()
                        {
                            Name = rec.Name,
                            HEAD_OPEN_PULSES = (float)rec.HEAD_OPEN_PULSES,
                            TURNS_BREAK = (float)rec.TURNS_BREAK,
                            PLC_PROG_NR = (ushort)rec.PLC_PROG_NR,
                            LOG_NO = (ushort)rec.LOG_NO,
                            Tq_UNIT = (ushort)rec.Tq_UNIT,
                            SelectedThreadType = (ThreadType)rec.SelectedThreadType,
                            Thread_step = (float)rec.Thread_step,
                            PIPE_TYPE = rec.PIPE_TYPE,

                            Box_Moni_Time = (int)rec.Box_Moni_Time,
                            Box_Len_Min = (float)rec.Box_Len_Min,
                            Box_Len_Max = (float)rec.Box_Len_Max,

                            Pre_Moni_Time = (int)rec.Pre_Moni_Time,
                            Pre_Len_Max = (float)rec.Pre_Len_Max,
                            Pre_Len_Min = (float)rec.Pre_Len_Min,

                            MU_Moni_Time = (int)rec.MU_Moni_Time,
                            MU_Tq_Ref = (float)rec.MU_Tq_Ref,
                            MU_Tq_Save = (float)rec.MU_Tq_Save,

                            JointMode = (JointMode)rec.SelectedMode,

                            MU_TqSpeedRed_1 = (float)rec.MU_TqSpeedRed_1,
                            MU_TqSpeedRed_2 = (float)rec.MU_TqSpeedRed_2,
                            MU_Tq_Dump = (float)rec.MU_Tq_Dump,
                            MU_Tq_Max = (float)rec.MU_Tq_Max,
                            MU_Tq_Min = (float)rec.MU_Tq_Min,
                            MU_Tq_Opt = (float)rec.MU_Tq_Opt,

                            MU_TqShoulder_Min = (float)rec.MU_TqShoulder_Min,
                            MU_TqShoulder_Max = (float)rec.MU_TqShoulder_Max,

                            MU_Len_Speed_1 = (float)rec.MU_Len_Speed_1,
                            MU_Len_Speed_2 = (float)rec.MU_Len_Speed_2,
                            MU_Len_Dump = (float)rec.MU_Len_Dump,
                            MU_Len_Min = (float)rec.MU_Len_Min,
                            MU_Len_Max = (float)rec.MU_Len_Max,

                            MU_JVal_Speed_1 = (float)rec.MU_JVal_Speed_1,
                            MU_JVal_Speed_2 = (float)rec.MU_JVal_Speed_2,
                            MU_JVal_Dump = (float)rec.MU_JVal_Dump,
                            MU_JVal_Min = (float)rec.MU_JVal_Min,
                            MU_JVal_Max = (float)rec.MU_JVal_Max,

                            TimeStamp = DateTime.Parse(rec.TimeStamp())
                        });                    
                }
            }

            return loadedRecipes;
        }


        //Операции над результатами

        public void SaveResult(JointResult result)
        {

        }

        public IEnumerable<JointResult> GetResults()
        {
            return new List<JointResult>();
        }
    }
}
