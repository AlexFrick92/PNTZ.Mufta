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
    public class RemoteRepository 
    {
        private ILogger _logger;        
        private LocalRepository _local;
        public RemoteRepository(ILogger logger)
        {
            _logger = logger;         
            
        }
        public void InitRepository()
        {
            using (var db = new RemoteRepositoryContext("PostgresDb"))
            {
                _logger.Info("Creatin remote repository tables");
                db.CreateTable<JointRecipeTable>(tableOptions: TableOptions.CheckExistence);
                db.CreateTable<JointResultTable>(tableOptions: TableOptions.CheckExistence);
            }
        }

        public void SyncRecipes()
        {

        }
    }
}
