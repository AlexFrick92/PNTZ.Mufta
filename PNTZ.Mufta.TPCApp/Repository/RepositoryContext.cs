
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
        public RepositoryContext(ILogger logger)
        {
            this.logger = logger;
            recipesConnectionString = $"Data Source={StoragePath}/RecipesData.db;Mode=ReadWriteCreate";

            using (var db = new JointRecipeContext(recipesConnectionString))
            {
                if (!db.DataProvider.GetSchemaProvider().GetSchema(db).Tables.Any(t => t.TableName == "Recipes"))
                {
                    //no required table-create it
                    db.CreateTable<JointRecipeTable>();
                    logger.Info("Создана таблица с рецептами.");
                }                
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

        }

        public IEnumerable<JointResult> GetResults()
        {
            return new List<JointResult>();
        }
    }
}
