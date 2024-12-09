
using Microsoft.Data.Sqlite;
using PNTZ.Mufta.TPCApp.Domain;

using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static PNTZ.Mufta.TPCApp.App;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public class RepositoryContext
    {
        ILogger logger;

        string StoragePath = App.AppInstance.CurrentDirectory + "/Repository";
        string conString;

        List<JointRecipe> loadedRecipes;

        public RepositoryContext(ILogger logger)
        {
            this.logger = logger;
            conString = $"Data Source={StoragePath}/processdata.db;Mode=ReadWriteCreate";

            if (!Directory.Exists(StoragePath))            
                Directory.CreateDirectory(StoragePath);            

            using (var connection = new SqliteConnection(conString))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand();

                command.Connection = connection;
                command.CommandText = SqlExpressions.CreateTable;

                command.ExecuteNonQuery();
            }
        }


        // ------------------- РЕЦЕПТЫ ---------------------------
        public void RemoveRecipe(JointRecipe recipe)
        {
            string sqlExpressionBase = $"DELETE FROM Recipes WHERE Name = '{recipe.Name}'";

            using (var connection = new SqliteConnection(conString))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand();
                command.Connection = connection;
                command.CommandText = sqlExpressionBase;
                command.ExecuteNonQuery();
            }

            logger.Info($"Рецепт {recipe.Name} удалён.");
        }
        public void SaveRecipe(JointRecipe rec)
        {
            if(loadedRecipes.FirstOrDefault(r => r.Name == rec.Name) != null)
                UpdateRecipe(rec);
            else
                InsertRecipe(rec);              

        }
        private void InsertRecipe(JointRecipe rec)
        {
            string sqlExpressionBase = SqlExpressions.InsertRecipe;
            string sqlValues = $"('{rec.Name}'," +
                    $"'{rec.HEAD_OPEN_PULSES}'," +
                    $"'{rec.TURNS_BREAK}'," +
                    $"'{rec.PLC_PROG_NR}'," +
                    $"'{rec.LOG_NO}'," +
                    $"'{rec.Tq_UNIT}'," +
                    $"'{(int)rec.SelectedThreadType}'," +
                    $"'{rec.Thread_step}'," +
                    $"'{rec.PIPE_TYPE}'," +

                    $"'{rec.Box_Moni_Time}'," +
                    $"'{rec.Box_Len_Min}'," +
                    $"'{rec.Box_Len_Max}'," +

                    $"'{rec.Pre_Moni_Time}'," +
                    $"'{rec.Pre_Len_Min}'," +
                    $"'{rec.Pre_Len_Max}'," +

                    $"'{rec.MU_Moni_Time}'," +
                    $"'{rec.MU_Tq_Ref}'," +
                    $"'{rec.MU_Tq_Save}'," +
                    $"'{(int)rec.JointMode}'," +
                    $"'{rec.MU_TqSpeedRed_1}'," +
                    $"'{rec.MU_TqSpeedRed_2}'," +
                    $"'{rec.MU_Tq_Dump}'," +
                    $"'{rec.MU_Tq_Max}'," +
                    $"'{rec.MU_Tq_Min}'," +
                    $"'{rec.MU_Tq_Opt}'," +
                    $"'{rec.MU_TqShoulder_Min}'," +
                    $"'{rec.MU_TqShoulder_Max}'," +

                    $"'{rec.MU_Len_Speed_1}'," +
                    $"'{rec.MU_Len_Speed_2}'," +
                    $"'{rec.MU_Len_Dump}'," +
                    $"'{rec.MU_Len_Min}'," +
                    $"'{rec.MU_Len_Max}'," +

                    $"'{rec.MU_Jval_Speed_1}'," +
                    $"'{rec.MU_Jval_Speed_2}'," +
                    $"'{rec.MU_Jval_Dump}'," +
                    $"'{rec.MU_Jval_Min}'," +
                    $"'{rec.MU_Jval_Max}'," +

                    $"'{rec.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}'" +

                    $");";
            using (var connection = new SqliteConnection(conString))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand();
                command.Connection = connection;
                command.CommandText = sqlExpressionBase + sqlValues;
                command.ExecuteNonQuery();
            }

            logger.Info($"Новый рецепт {rec.Name} сохранён.");
        }
        private void UpdateRecipe(JointRecipe rec)
        {
            string sqlExpressionBase = "UPDATE Recipes SET ";
            string sqlValues = 
                    $"HEAD_OPEN_PULSES = '{rec.HEAD_OPEN_PULSES}'," +
                    $"TURNS_BREAK = '{rec.TURNS_BREAK}'," +
                    $"PLC_PROG_NR = '{rec.PLC_PROG_NR}'," +
                    $"LOG_NO = '{rec.LOG_NO}'," +
                    $"Tq_UNIT = '{rec.Tq_UNIT}'," +
                    $"SelectedThreadType = '{(int)rec.SelectedThreadType}'," +
                    $"Thread_step = '{rec.Thread_step}'," +
                    $"PIPE_TYPE = '{rec.PIPE_TYPE}'," +

                    $"Box_Moni_Time = '{rec.Box_Moni_Time}'," +
                    $"Box_Len_Min = '{rec.Box_Len_Min}'," +
                    $"Box_Len_Min = '{rec.Box_Len_Max}'," +

                    $"Pre_Moni_Time = '{rec.Pre_Moni_Time}'," +
                    $"Pre_Len_Min = '{rec.Pre_Len_Min}'," +
                    $"Pre_Len_Max = '{rec.Pre_Len_Max}'," +

                    $"MU_Moni_Time = '{rec.MU_Moni_Time}'," +
                    $"MU_Tq_Ref = '{rec.MU_Tq_Ref}'," +
                    $"MU_Tq_Save = '{rec.MU_Tq_Save}'," +
                    $"SelectedMode = '{(int)rec.JointMode}'," +
                    $"MU_TqSpeedRed_1 = '{rec.MU_TqSpeedRed_1}'," +
                    $"MU_TqSpeedRed_2 = '{rec.MU_TqSpeedRed_2}'," +
                    $"MU_Tq_Dump = '{rec.MU_Tq_Dump}'," +
                    $"MU_Tq_Max = '{rec.MU_Tq_Max}'," +
                    $"MU_Tq_Min = '{rec.MU_Tq_Min}'," +
                    $"MU_Tq_Opt = '{rec.MU_Tq_Opt}'," +
                    $"MU_TqShoulder_Min = '{rec.MU_TqShoulder_Min}'," +
                    $"MU_TqShoulder_Max = '{rec.MU_TqShoulder_Max}'," +

                    $"MU_Len_Speed_1 = '{rec.MU_Len_Speed_1}'," +
                    $"MU_Len_Speed_2 = '{rec.MU_Len_Speed_2}'," +
                    $"MU_Len_Dump = '{rec.MU_Len_Dump}'," +
                    $"MU_Len_Min = '{rec.MU_Len_Min}'," +
                    $"MU_Len_Max = '{rec.MU_Len_Max}'," +

                    $"MU_JVal_Speed_1 = '{rec.MU_Jval_Speed_1}'," +
                    $"MU_Jval_Speed_2 = '{rec.MU_Jval_Speed_2}'," +
                    $"MU_JVal_Dump = '{rec.MU_Jval_Dump}'," +
                    $"MU_JVal_Min = '{rec.MU_Jval_Min}'," +
                    $"MU_JVal_Max = '{rec.MU_Jval_Max}'," +

                    $"TimeStamp = '{rec.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}'" +

                    $" WHERE Name = '{rec.Name}';";
            using (var connection = new SqliteConnection(conString))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand();
                command.Connection = connection;
                command.CommandText = sqlExpressionBase + sqlValues;
                command.ExecuteNonQuery();
            }

            logger.Info($"Рецепт {rec.Name} обновлён.");
        }

        
        public void SaveRecipes(IEnumerable<JointRecipe> jointRecipes)
        {
            string sqlExpressionBase = SqlExpressions.InsertRecipe;

            string sqlValues = "";

            foreach (JointRecipe jr in jointRecipes)
            {
                sqlValues += $"('{jr.Name}'," +
                    $"'{jr.HEAD_OPEN_PULSES}'," +
                    $"'{jr.TURNS_BREAK}'," +
                    $"'{jr.PLC_PROG_NR}'," +
                    $"'{jr.LOG_NO}'," +
                    $"'{jr.Tq_UNIT}'," +
                    $"'{(int)jr.SelectedThreadType}'," +
                    $"'{jr.Thread_step}'," +
                    $"'{jr.PIPE_TYPE}'," +

                    $"'{jr.Box_Moni_Time}'," +
                    $"'{jr.Box_Len_Min}'," +
                    $"'{jr.Box_Len_Max}'," +

                    $"'{jr.Pre_Moni_Time}'," +
                    $"'{jr.Pre_Len_Min}'," +
                    $"'{jr.Pre_Len_Max}'," +

                    $"'{jr.MU_Moni_Time}'," +
                    $"'{jr.MU_Tq_Ref}'," +
                    $"'{jr.MU_Tq_Save}'," +
                    $"'{(int)jr.JointMode}'," +
                    $"'{jr.MU_TqSpeedRed_1}'," +
                    $"'{jr.MU_TqSpeedRed_2}'," +
                    $"'{jr.MU_Tq_Dump}'," +
                    $"'{jr.MU_Tq_Max}'," +
                    $"'{jr.MU_Tq_Min}'," +
                    $"'{jr.MU_Tq_Opt}'," +
                    $"'{jr.MU_TqShoulder_Min}'," +
                    $"'{jr.MU_TqShoulder_Max}'," +

                    $"'{jr.MU_Len_Speed_1}'," +
                    $"'{jr.MU_Len_Speed_2}'," +
                    $"'{jr.MU_Len_Dump}'," +
                    $"'{jr.MU_Len_Min}'," +
                    $"'{jr.MU_Len_Max}'," +

                    $"'{jr.MU_Jval_Speed_1}'," +
                    $"'{jr.MU_Jval_Speed_2}'," +
                    $"'{jr.MU_Jval_Dump}'," +
                    $"'{jr.MU_Jval_Speed_1}'," +
                    $"'{jr.MU_Jval_Speed_2}'," +

                    $"'{jr.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}'" +

                    $"),";
            }

            sqlValues = sqlValues.Remove(sqlValues.Length -1) +  ";";

            using (var connection = new SqliteConnection(conString))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand();
                command.Connection = connection;
                command.CommandText = sqlExpressionBase + sqlValues;
                command.ExecuteNonQuery();
            }

            logger.Info("Сохранено");

        }
        public IEnumerable<JointRecipe> LoadRecipes()
        {
            loadedRecipes = new List<JointRecipe> ();
            logger.Info("Загружаем рецепты...");

            using (var connection = new SqliteConnection(conString))
            {
                connection.Open();

                string selectQuery = SqlExpressions.SelectRecipes;
                SqliteCommand command = new SqliteCommand(selectQuery);
                command.Connection = connection;    
                command.CommandText = selectQuery;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        loadedRecipes.Add(new JointRecipe() { 
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            HEAD_OPEN_PULSES = (float)reader.GetDouble(reader.GetOrdinal("HEAD_OPEN_PULSES")),
                            TURNS_BREAK = (float)reader.GetDouble(reader.GetOrdinal("TURNS_BREAK")),
                            PLC_PROG_NR = (ushort)reader.GetInt16(reader.GetOrdinal("PLC_PROG_NR")),
                            LOG_NO = (ushort)reader.GetInt16(reader.GetOrdinal("LOG_NO")),
                            Tq_UNIT = (ushort)reader.GetInt16(reader.GetOrdinal("Tq_UNIT")),
                            SelectedThreadType = (ThreadType)reader.GetInt32(reader.GetOrdinal("SelectedThreadType")),
                            Thread_step = (float)reader.GetDouble(reader.GetOrdinal("Thread_step")),

                            Box_Moni_Time = (ushort)reader.GetInt16(reader.GetOrdinal("Box_Moni_Time")),
                            Box_Len_Min = (ushort)reader.GetInt16(reader.GetOrdinal("Box_Len_Min")),
                            Box_Len_Max = (ushort)reader.GetInt16(reader.GetOrdinal("Box_Len_Max")),

                            Pre_Moni_Time = (ushort)reader.GetInt16(reader.GetOrdinal("Pre_Moni_Time")),
                            Pre_Len_Min = (float)reader.GetDouble(reader.GetOrdinal("Pre_Len_Min")),
                            Pre_Len_Max = (float)reader.GetDouble(reader.GetOrdinal("Pre_Len_Max")),

                            MU_Moni_Time = (ushort)reader.GetInt16(reader.GetOrdinal("MU_Moni_Time")),
                            MU_Tq_Ref = (float)reader.GetDouble(reader.GetOrdinal("MU_Tq_Ref")),
                            MU_Tq_Save = (float)reader.GetDouble(reader.GetOrdinal("MU_Tq_Save")),

                            JointMode = (JointMode)reader.GetInt32(reader.GetOrdinal("SelectedMode")),

                            MU_TqSpeedRed_1 = (float)reader.GetDouble(reader.GetOrdinal("MU_TqSpeedRed_1")),
                            MU_TqSpeedRed_2 = (float)reader.GetDouble(reader.GetOrdinal("MU_TqSpeedRed_2")),

                            MU_Tq_Dump = (float)reader.GetDouble(reader.GetOrdinal("MU_Tq_Dump")),
                            MU_Tq_Max = (float)reader.GetDouble(reader.GetOrdinal("MU_Tq_Max")),
                            MU_Tq_Min = (float)reader.GetDouble(reader.GetOrdinal("MU_Tq_Min")),
                            MU_Tq_Opt = (float)reader.GetDouble(reader.GetOrdinal("MU_Tq_Opt")),

                            MU_TqShoulder_Min = (float)reader.GetDouble(reader.GetOrdinal("MU_TqShoulder_Min")),
                            MU_TqShoulder_Max = (float)reader.GetDouble(reader.GetOrdinal("MU_TqShoulder_Max")),

                            MU_Len_Speed_1 = (float)reader.GetDouble(reader.GetOrdinal("MU_Len_Speed_1")),
                            MU_Len_Speed_2 = (float)reader.GetDouble(reader.GetOrdinal("MU_Len_Speed_2")),
                            MU_Len_Dump = (float)reader.GetDouble(reader.GetOrdinal("MU_Len_Dump")),
                            MU_Len_Min = (float)reader.GetDouble(reader.GetOrdinal("MU_Len_Min")),
                            MU_Len_Max = (float)reader.GetDouble(reader.GetOrdinal("MU_Len_Max")),

                            MU_Jval_Speed_1 = (float)reader.GetDouble(reader.GetOrdinal("MU_Len_Max")),
                            MU_Jval_Speed_2 = (float)reader.GetDouble(reader.GetOrdinal("MU_Len_Max")),
                            MU_Jval_Dump = (float)reader.GetDouble(reader.GetOrdinal("MU_Len_Max")),
                            MU_Jval_Min = (float)reader.GetDouble(reader.GetOrdinal("MU_Len_Max")),
                            MU_Jval_Max = (float)reader.GetDouble(reader.GetOrdinal("MU_Len_Max")),

                            TimeStamp = DateTime.Parse(reader.GetString(reader.GetOrdinal("TimeStamp"))),
                        });;
                    }                
                }
            }

            logger.Info("Рецепты загружены.");
            return loadedRecipes;
        }


        // ------------------------- СВИНЧИВАНИЯ ---------------------

        List<JointResult> Joints = new List<JointResult>();

        public void SaveResult(JointResult result)
        {
            Joints.Add(result);
            logger.Info("Результат соединения сохранён.");
        }

        public IEnumerable<JointResult> GetResults()
        {
            logger.Info("Загрузка результатов...");
            logger.Info("Результаты загружены " + Joints.Count);
            return Joints;
        }

        

    }
}
