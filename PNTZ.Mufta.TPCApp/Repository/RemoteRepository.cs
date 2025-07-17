using LinqToDB;
using LinqToDB.Data;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public class RemoteRepository : IDisposable
    {
        private ILogger _logger;        
        private LocalRepository _local;
        private string config = "PostgresDb";

        private RemoteRepositoryContext _context;
        public RemoteRepository(LocalRepository local, ILogger logger)
        {
            _logger = logger;
            _local = local;

            _context = new RemoteRepositoryContext(config);            
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public void InitRepository()
        {
            _logger.Info("Creating remote repository tables");
            _context.CreateTable<JointRecipeTable>(tableOptions: TableOptions.CheckExistence);
            _context.CreateTable<JointResultTable>(tableOptions: TableOptions.CheckExistence);
        }

        public void SyncRecipes()
        {
            _logger.Info("Syncing recipes...");
            var remoteRecipes = _context.Recipes.ToList();
            _local.LoadRecipes(remoteRecipes);

            var localRecipes = _local.RecipesTable.ToList();
            SyncRemoteRecipes(localRecipes);

            _logger.Info("Recipes is synced");
        }

        public void SyncRemoteRecipes(IEnumerable<JointRecipeTable> recipes)
        {
            foreach (var recipe in recipes)
            {
                var recToUpdate = _context.Recipes.FirstOrDefault(r => r.Name == recipe.Name);

                if (recToUpdate != null)
                {
                    if (recToUpdate.TimeStamp < recipe.TimeStamp)
                    {
                        recToUpdate.CopyProperties(recipe);
                        _context.Update(recToUpdate);
                        _logger.Info($"Рецепт {recipe.Name} обновлён в удалённом репозитории.");
                    }
                }
                else
                {
                    _context.Insert(recipe);
                    _logger.Info($"Рецепт {recipe.Name} добавлен в удалённый репозиторий.");
                }
            }
        }
    }
}
