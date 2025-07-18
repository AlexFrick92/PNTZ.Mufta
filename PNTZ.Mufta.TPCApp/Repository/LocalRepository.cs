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
        private RemoteRepository _remoteRepo;
        ILogger _logger;
        string StoragePath = App.AppInstance.CurrentDirectory + "/Repository";
        string recipesConnectionString;
        string resultsConnectionString;
        public LocalRepository(ILogger logger)
        {
            this._logger = logger;
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

            _remoteRepo = new RemoteRepository(logger);
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
                    _logger.Info($"Рецепт {recipe.Name} обновлён.");
                }
                else
                {
                    db.Insert(new JointRecipeTable().FromJointRecipe(recipe));
                    _logger.Info($"Рецепт {recipe.Name} создан.");
                }
            }
        }
        public void SyncRecipes()
        {            
            using(var db = new JointRecipeContext(recipesConnectionString))
            {
                var remoteRecipes = _remoteRepo.GetRecipes();

                foreach(var remoteRecipe in remoteRecipes)
                {
                    var recToUpdate = db.Recipes.FirstOrDefault(r => r.Name == remoteRecipe.Name);

                    if (recToUpdate != null)
                    {                        
                        if(recToUpdate.TimeStamp < remoteRecipe.TimeStamp)
                        {
                            recToUpdate.CopyProperties(remoteRecipe);
                            db.Update(recToUpdate);
                            _logger.Info($"Рецепт {remoteRecipe.Name} обновлён.");
                        }
                    }
                    else
                    {
                        db.Insert(remoteRecipe);
                        _logger.Info($"Рецепт {remoteRecipe.Name} добавлен.");
                    }
                }

                _remoteRepo.SyncRemoteRecipes(GetRecipes());
            }
        }
        public void RemoveRecipe(JointRecipe recipe)
        {
            using (var db = new JointRecipeContext(recipesConnectionString))
            {
                var recToUpdate = db.Recipes.FirstOrDefault(r => r.Name == recipe.Name);

                db.Delete(recToUpdate);
                _logger.Info($"Рецепт {recipe.Name} удалён.");
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
                _logger.Info($"Соединение {row.Name} сохранёно.");
            }
        }

        public void InitRepository() => _remoteRepo.InitRepository();        
        public void UploadResults()
        {
            _logger.Info($"Uploading results...");
            using (var db = new JointResultContext(resultsConnectionString))
            {
                var results = db.Results.AsEnumerable().ToList();

                _logger.Info($"Results to upload : {results.Count}");

                try
                {
                    _remoteRepo.UploadResult(results);
                    db.Results.Delete();

                    _logger.Info("Uploaded. Locally deleted");
                    
                }
                catch (Exception ex)
                {
                    _logger.Error($"Upload result failed : {ex.Message}");
                }                
            }
        }
    }
}
