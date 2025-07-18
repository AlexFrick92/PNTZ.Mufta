using LinqToDB;
using LinqToDB.Data;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public class RemoteRepository
    {
        private ILogger _logger;                
        private string config = "PostgresDb";

        public RemoteRepository(ILogger logger)
        {
            _logger = logger;                               
        }

        public void InitRepository()
        {
            using (var db = new RemoteRepositoryContext(config))
            {
                _logger.Info("Creating remote repository tables");
                db.CreateTable<JointRecipeTable>(tableOptions: TableOptions.CheckExistence);
                db.CreateTable<JointResultTable>(tableOptions: TableOptions.CheckExistence);
            }
        }
        public void SyncRemoteRecipes(IEnumerable<JointRecipeTable> recipes)
        {
            using (var db = new RemoteRepositoryContext(config))
            {
                foreach (var recipe in recipes)
                {
                    var recToUpdate = db.Recipes.FirstOrDefault(r => r.Name == recipe.Name);

                    if (recToUpdate != null)
                    {
                        if (recToUpdate.TimeStamp < recipe.TimeStamp)
                        {
                            recToUpdate.CopyProperties(recipe);
                            db.Update(recToUpdate);
                            _logger.Info($"Рецепт {recipe.Name} обновлён в удалённом репозитории.");
                        }
                    }
                    else
                    {
                        db.Insert(recipe);
                        _logger.Info($"Рецепт {recipe.Name} добавлен в удалённый репозиторий.");
                    }
                }
            }
        }
        public List<JointRecipeTable> GetRecipes(Expression<Func<JointRecipeTable, bool>> filter = null)
        {
            using (var db = new RemoteRepositoryContext(config))
            {
                var query = db.Recipes.AsQueryable();

                if (filter != null)
                    query = query.Where(filter);

                return query.ToList();
            }
        }
        public List<JointResultTable> GetResults(Expression<Func<JointResultTable, bool>> filter = null)
        {
            using (var db = new RemoteRepositoryContext(config))
            {
                var query = db.Results.AsQueryable();

                if (filter != null)
                    query = query.Where(filter);

                return query.ToList();
            }
        }
        public void UploadResult(IEnumerable<JointResultTable> results)
        {
            using (var db = new RemoteRepositoryContext(config))
            {
                foreach (var result in results)
                {
                    var existsResult = db.Results.FirstOrDefault(r => r.Id == result.Id);
                    if (existsResult == null)
                        db.Insert(result);
                }
            }
        }
    }
}
