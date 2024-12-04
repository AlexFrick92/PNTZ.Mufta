
using Microsoft.Data.Sqlite;
using PNTZ.Mufta.TPCApp.Domain;

using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using static PNTZ.Mufta.TPCApp.App;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public class RepositoryContext
    {
        ILogger logger;

        string StoragePath = App.AppInstance.CurrentDirectory + "/Repository";
        string conString;

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

                    $"'{jr.TimeStamp}'" +

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
            List<JointRecipe> recipes = new List<JointRecipe> ();
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
                        recipes.Add(new JointRecipe() { 
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

                            JointMode = (JointMode)reader.GetInt32(reader.GetOrdinal("JointMode")),

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
            return recipes;
        }
    }
}
