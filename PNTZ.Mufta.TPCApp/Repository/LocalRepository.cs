using PNTZ.Mufta.TPCApp.Domain;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;

using System.Data.SQLite;
using System.Linq.Expressions;
using Promatis.Core.Extensions;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public class LocalRepository
    {
        private RemoteRepository _remoteRepo;
        ILogger _logger;
        string StoragePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PNTZ.Mufta.TPCApp",
            "Repository"
        );
        string recipesConnectionString;
        string resultsConnectionString;
        public LocalRepository(ILogger logger)
        {
            this._logger = logger;

            // Создаём папку Repository в AppData\Local, если её нет
            if (!System.IO.Directory.Exists(StoragePath))
            {
                System.IO.Directory.CreateDirectory(StoragePath);
                _logger.Info($"Создана папка {StoragePath}");
            }

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

        public void SaveRecipe(JointRecipeTable recipe)
        {            
            using (var db = new JointRecipeContext(recipesConnectionString))
            {
                // Ищем по Id, чтобы понять, это новый рецепт или существующий
                var recToUpdate = db.Recipes.FirstOrDefault(r => r.Id == recipe.Id);

                if (recToUpdate != null)
                {
                    // Копируем все свойства из recipe в найденную запись из БД
                    recToUpdate.CopyProperties(recipe);
                    recToUpdate.TimeStamp = DateTime.UtcNow;
                    db.Update(recToUpdate);
                    _logger.Info($"Рецепт {recipe.Name} обновлён.");
                }
                else
                {
                    recipe.TimeStamp = DateTime.UtcNow;
                    db.Insert(recipe);
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
                        if(remoteRecipe.RemovedDate != null)
                        {
                            db.Delete(recToUpdate);
                            _logger.Info($"Рецепт {remoteRecipe.Name} удалён из локальной базы данных.");
                        }
                        else if(recToUpdate.TimeStamp < remoteRecipe.TimeStamp)
                        {
                            recToUpdate.CopyProperties(remoteRecipe);
                            db.Update(recToUpdate);
                            _logger.Info($"Рецепт {remoteRecipe.Name} обновлён.");
                        }
                    }
                    else
                    {
                        if (remoteRecipe.RemovedDate == null)
                        {
                            db.Insert(remoteRecipe);
                            _logger.Info($"Рецепт {remoteRecipe.Name} добавлен.");
                        }
                    }
                }

                _remoteRepo.SyncRemoteRecipes(GetRecipes());
                db.Recipes.Where(r => r.RemovedDate != null).Delete();                    
            }
        }
        public void RemoveRecipe(JointRecipe recipe)
        {
            using (var db = new JointRecipeContext(recipesConnectionString))
            {
                var recToUpdate = db.Recipes.FirstOrDefault(r => r.Id == recipe.Id);
                _logger.Info($"{recipe.Id}");
                if(recToUpdate == null)
                {
                    _logger.Error($"Рецепт {recipe.Name} не найден в локальной базе данных.");
                    return;
                }
                else
                {
                    recToUpdate.TimeStamp = DateTime.Now;
                    recToUpdate.RemovedDate = DateTime.Now;
                    db.Update(recToUpdate);
                    _logger.Info($"Рецепт {recipe.Name} удалён.");
                }                
            }
        }
        public List<JointRecipeTable> GetRecipes(Expression<Func<JointRecipeTable, bool>> filter = null)
        {
            using (var db = new JointRecipeContext(recipesConnectionString))
            {                
                var query = db.Recipes.AsQueryable();

                if (filter != null)
                    query = query.Where(r => r.RemovedDate == null).Where(filter);

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

        /// <summary>
        /// Получить результат по ID
        /// </summary>
        /// <param name="id">ID результата</param>
        /// <returns>JointResult или null</returns>
        public JointResult GetResultById(Guid id)
        {
            using (var db = new JointResultContext(resultsConnectionString))
            {
                var resultTable = db.Results.FirstOrDefault(r => r.Id == id);
                return resultTable?.ToJointResult();
            }
        }
        public DateTime GetFirstDateTime(string recipeName)
        {
            using (var db = new JointResultContext(resultsConnectionString))            
                return db.Results.AsQueryable().Where(r => r.Name == recipeName).Min(q => q.FinishTimeStamp);
        }
        public DateTime GetLastDateTime(string recipeName)
        {
            using (var db = new JointResultContext(resultsConnectionString))
                return db.Results.AsQueryable().Where(r => r.Name == recipeName).Max(q => q.FinishTimeStamp);
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
        public void PushResults()
        {
            _logger.Info($"Uploading results...");
            using (var db = new JointResultContext(resultsConnectionString))
            {
                var results = db.Results.AsEnumerable().ToList();

                _logger.Info($"Results to upload : {results.Count}");

                _remoteRepo.UploadResult(results);
                //db.Results.Delete();

                _logger.Info("Uploaded");                                 
            }
        }
        //Временный метод для загрузки результатов из удалённого репозитория
        //В дальнейшем, локально мы не будем хранить результаты
        public void PullResults(string recipeName)
        {
            _logger.Info($"Downloading results...");
            using (var db = new JointResultContext(resultsConnectionString))
            {
                int i = 0;
                
                if(recipeName.IsNotEmpty())
                {
                    foreach (var remoteResult in _remoteRepo.GetResults(r => r.Name == recipeName))
                    {
                        if (db.Results.FirstOrDefault(r => r.Id == remoteResult.Id) == null)
                        {
                            db.Insert(remoteResult);
                            i++;
                        }
                    }
                    _logger.Info($"Downloaded {i} results for recipe name '{recipeName}'");
                }
                else
                {
                    foreach (var remoteResult in _remoteRepo.GetResults())
                    {
                        if (db.Results.FirstOrDefault(r => r.Id == remoteResult.Id) == null)
                        {
                            db.Insert(remoteResult);
                            i++;
                        }
                    }
                    _logger.Info($"Downloaded {i} results");
                }
            }
        }        

        public void ClearLocalResults()
        {
            _logger.Info($"Clearing results...");
            using (var db = new JointResultContext(resultsConnectionString))
            {
                int count = db.Results.Count();
                db.Results.Delete();
                _logger.Info($"Removed {count} from local repository");
            }
        }

        public List<string> FetchRemoteResultsNames()
        {
            _logger.Info($"Remote recipe names: ");
            var remoteNames = _remoteRepo.GetResultsRecipes();
            foreach (var name in remoteNames)
            {
                _logger.Info(name);
            }
            _logger.Info($"***");
            return remoteNames;
        }

    }
}
