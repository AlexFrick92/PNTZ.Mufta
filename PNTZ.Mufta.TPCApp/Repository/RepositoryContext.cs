
using PNTZ.Mufta.TPCApp.Domain;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using System.Data.SQLite;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public class RepositoryContext
    {
        ILogger logger;
        string StoragePath = App.AppInstance.CurrentDirectory + "/Repository";
        string recipesConnectionString;

        List<JointRecipe> loadedRecipes = new List<JointRecipe> ();
        public RepositoryContext(ILogger logger)
        {
            this.logger = logger;
            recipesConnectionString = $"Data Source={StoragePath}/RecipesData.db;Mode=ReadWriteCreate";

            using (var db = new JointRecipeContext(recipesConnectionString))
            {
                try
                {
                    db.CreateTable<JointRecipeTable>();
                    logger.Info("Создана таблица с рецептами.");
                }
                catch (SQLiteException ex)
                {
                    logger.Info("Таблица с рецептами уже создана");
                }
            }


        }
        public void SaveRecipe(JointRecipe rec)
        {

            InsertRecipe(rec);
        }

        private void InsertRecipe(JointRecipe recipe)
        {
            using (var db = new JointRecipeContext(recipesConnectionString))
            {
                db.Insert(new JointRecipeTable().FromJointRecipe(recipe));
            }
        }


        public IEnumerable<JointRecipe> LoadRecipes()
        {
            loadedRecipes.Clear();
            using (var db = new JointRecipeContext(recipesConnectionString))
            {
                foreach (var rec in db.Recipes.ToList())
                {
                    loadedRecipes.Add(rec.ToJointRecipe());
                }
            }
            return loadedRecipes;
        }

        public void RemoveRecipe(JointRecipe recipe)
        {

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
