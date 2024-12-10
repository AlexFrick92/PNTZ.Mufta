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
    public class RepositoryContext1
    {

        ILogger logger;

        string StoragePath = App.AppInstance.CurrentDirectory + "/Repository";
        readonly string recipeTableName = "Recipes";
        string recipesConnectionString;

        List<JointRecipe> loadedRecipes = new List<JointRecipe> ();

        public RepositoryContext1(ILogger logger)
        {
            this.logger = logger;
            recipesConnectionString = $"Data Source={StoragePath}/RecipesData.db;Mode=ReadWriteCreate";

            CreateTable(recipeTableName, recipesConnectionString);

            LoadRecipes();

        }
        private void CreateTable(string tableName, string connectionString)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var createTableQuery = SqlQueriesGenerator.CreateTable<JointRecipeMapper>(tableName);
                connection.Execute(createTableQuery);
            }
        }       


        //Операции над рецептами

        public void SaveRecipe(JointRecipe rec)
        {

            if (loadedRecipes.FirstOrDefault(r => r.Name == rec.Name) != null)
                UpdateRecipe(rec);
            else
                InsertRecipe(rec);            
        }
        private void InsertRecipe(JointRecipe recipe)
        {
            using (var connection = new SqliteConnection(recipesConnectionString))
            {
                connection.Open();
                var insertQuery = SqlQueriesGenerator.Insert<JointRecipeMapper>(recipeTableName);                  

 
                var mapper = new JointRecipeMapper().FromJointRecipe(recipe);
                connection.Execute(insertQuery,  mapper);
                logger.Info($"Рецепт {recipe.Name} создан.");
            };
        }        
        private void UpdateRecipe(JointRecipe recipe)
        {
            using (var connection = new SqliteConnection(recipesConnectionString))
            {
                connection.Open();
                var updateQuery = SqlQueriesGenerator.Update<JointRecipeMapper>(recipeTableName, "WHERE Name = @Name");                
                var mapper = new JointRecipeMapper().FromJointRecipe(recipe);

                connection.Execute(updateQuery, mapper);
                logger.Info($"Рецепт {recipe.Name} обновлён.");
            };
        }        
        public void RemoveRecipe(JointRecipe recipe)
        {
            using (var connection = new SqliteConnection(recipesConnectionString))
            {
                connection.Open();
                var deleteQuery = $"DELETE FROM {recipeTableName} WHERE Name = @Name;";
                var param = new {Name =  recipe.Name};
                connection.Execute(deleteQuery, param);
                logger.Info($"Рецепт {recipe.Name} удалён.");
            }
        }
        public IEnumerable<JointRecipe> LoadRecipes()
        {            
            using (var connection = new SqliteConnection(recipesConnectionString))
            {
                connection.Open();
                var selectQuery = SqlQueriesGenerator.SelectFrom<JointRecipeMapper>(recipeTableName);
                var recipesFromDb = connection.Query<JointRecipeMapper>(selectQuery).ToList();

                loadedRecipes.Clear();

                foreach (JointRecipeMapper rec in recipesFromDb)
                {
                    loadedRecipes.Add(
                        rec.ToJointRecipe()
                       );
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
