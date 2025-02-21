
using PNTZ.Mufta.TPCApp.Domain;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;


using LinqToDB;

using System.Data.SQLite;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public class RepositoryContext
    {
        ILogger logger;
        string StoragePath = App.AppInstance.CurrentDirectory + "/Repository";
        string recipesConnectionString;
        string resultsConnectionString;
        public RepositoryContext(ILogger logger)
        {
            this.logger = logger;
            recipesConnectionString = $"Data Source={StoragePath}/RecipesData.db;Mode=ReadWriteCreate";
            resultsConnectionString = $"Data Source={StoragePath}/ResultsData.db;Mode=ReadWriteCreate";

            using (var db = new JointRecipeContext(recipesConnectionString))
            {                              
                db.CreateTable<JointRecipeTable>(tableOptions: TableOptions.CheckExistence);              
            }

            using (var db = new JointResultContext(resultsConnectionString))
            {
                db.CreateTable<JointResultTable>(tableOptions: TableOptions.CheckExistence);
            }
        }
        public void SaveRecipe(JointRecipe recipe)
        {
            using (var db = new JointRecipeContext(recipesConnectionString))
            {
                var recToUpdate = db.Recipes.FirstOrDefault(r => r.Name == recipe.Name);

                if(recToUpdate != null)
                {
                    recToUpdate.FromJointRecipe(recipe);
                    db.Update(recToUpdate);
                    logger.Info($"Рецепт {recipe.Name} обновлён.");
                }
                else
                {
                    db.Insert(new JointRecipeTable().FromJointRecipe(recipe));
                    logger.Info($"Рецепт {recipe.Name} создан.");
                }
            }
        }
        public void RemoveRecipe(JointRecipe recipe)
        {
            using (var db = new JointRecipeContext(recipesConnectionString))
            {
                var recToUpdate = db.Recipes.FirstOrDefault(r => r.Name == recipe.Name);

                db.Delete(recToUpdate);
                logger.Info($"Рецепт {recipe.Name} удалён.");
            }
        }
        public IEnumerable<JointRecipe> LoadRecipes()
        {            
            using (var db = new JointRecipeContext(recipesConnectionString))
            {
                return db.Recipes.ToList().Select(recTable => recTable.ToJointRecipe());
            }

        }

        //Операции над результатами

        public void SaveResult(JointResult result)
        {
            using (var db = new JointResultContext(resultsConnectionString))
            {
                JointResultTable row = new JointResultTable().FromJointResult(result);
                db.Insert(row);
                logger.Info($"Соединение {row.Name} сохранёно.");
            }
        }

        public IEnumerable<JointResult> LoadResults()
        {
            using (var db = new JointResultContext(resultsConnectionString))
            {
                List<JointResult> resultList = new List<JointResult> ();
                foreach(var row in db.GetResultPage(1, 100).ToList())
                {
                    try
                    {
                        resultList.Add( row.ToJointResult() );
                    }
                    catch (Exception ex) 
                    {
                        logger.Error("Не удалось достать результат из базы: " + ex.Message);
                    }
                }

                return resultList;
            }
        }
    }
}
