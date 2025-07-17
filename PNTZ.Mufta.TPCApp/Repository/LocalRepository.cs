
using PNTZ.Mufta.TPCApp.Domain;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;


using LinqToDB;

using System.Data.SQLite;
using System.Linq.Expressions;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public class LocalRepository
    {
        ILogger logger;
        string StoragePath = App.AppInstance.CurrentDirectory + "/Repository";
        string recipesConnectionString;
        string resultsConnectionString;
        public LocalRepository(ILogger logger)
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
        public void LoadRecipes(IEnumerable<JointRecipeTable> recipes)
        {
            using(var db = new JointRecipeContext(recipesConnectionString))
            {
                foreach(var recipe in recipes)
                {
                    var recToUpdate = db.Recipes.FirstOrDefault(r => r.Name == recipe.Name);

                    if (recToUpdate != null)
                    {                        
                        if(recToUpdate.TimeStamp < recipe.TimeStamp)
                        {
                            recToUpdate.CopyProperties(recipe);
                            db.Update(recToUpdate);
                            logger.Info($"Рецепт {recipe.Name} обновлён.");
                        }
                    }
                    else
                    {
                        db.Insert(recipe);
                        logger.Info($"Рецепт {recipe.Name} добавлен.");
                    }
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
        public List<JointRecipeTable> GetRecipes(Expression<Func<JointRecipeTable, bool>> filter = null)
        {
            using (var db = new JointRecipeContext(recipesConnectionString))
            {                
                var query = db.Recipes.AsQueryable();

                if (filter != null)
                    query = query.Where(filter);

                return query.ToList();
            }
        }
        public List<JointResultTable> GetResults(Expression<Func<JointResultTable, bool>> filter = null)
        {
            using (var db = new JointResultContext(resultsConnectionString))
            {
                var query = db.Results.AsQueryable();

                if (filter != null)
                    query = query.Where(filter);

                return query.ToList();
            }
        }
        public List<string> GetResultsRecipes(Expression<Func<JointResultTable, bool>> filter = null)
        {
            using (var db = new JointResultContext(resultsConnectionString))
            {
                var query = db.Results.AsQueryable();

                if (filter != null)
                    query = query.Where(filter);

                return query.Select(r => r.Name).Distinct().ToList();
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
    }
}
