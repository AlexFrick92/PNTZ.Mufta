using LinqToDB;
using LinqToDB.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public class RemoteRepositoryContext : DataConnection
    {
        public RemoteRepositoryContext(string configName) : base(configName)
        {

        }

        public ITable<JointResultTable> Results => this.GetTable<JointResultTable>();
        public ITable<JointRecipeTable> Recipes => this.GetTable<JointRecipeTable>();
    }
}
