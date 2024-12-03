
using Microsoft.Data.Sqlite;
using PNTZ.Mufta.TPCApp.Domain;

using Promatis.Core.Logging;

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
                command.CommandText = "CREATE TABLE IF NOT EXISTS Recipes(Name TEXT)";

                command.ExecuteNonQuery();
            }
        }


        public void SaveRecipes(IEnumerable<JointRecipe> jointRecipes)
        {
            string sqlExpressionBase = $"INSERT INTO Recipes (Name) VALUES ";

            string sqlValues = "";

            foreach (JointRecipe jointRecipe in jointRecipes)
            {
                sqlValues += $"('{jointRecipe.Name}'),";
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

                string selectQuery = "SELECT Name FROM Recipes;";
                SqliteCommand command = new SqliteCommand(selectQuery);
                command.Connection = connection;    
                command.CommandText = selectQuery;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        recipes.Add(new JointRecipe() { Name = reader.GetString(reader.GetOrdinal("Name")) });

                    }                
                }
            }

            logger.Info("Рецепты загружены.");
            return recipes;
        }
    }
}
