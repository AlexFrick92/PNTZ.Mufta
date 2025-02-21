using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public class JointResultContext : DataConnection
    {
        public JointResultContext(string connectionString) : base(ProviderName.SQLite, connectionString)
        {

        }

        public ITable<JointResultTable> Results => this.GetTable<JointResultTable>();

        public IEnumerable<JointResultTable> GetResultPage(int page, int pageSize = 100)
        {
            return Results.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
