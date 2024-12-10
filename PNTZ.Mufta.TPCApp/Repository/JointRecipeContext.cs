using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;


namespace PNTZ.Mufta.TPCApp.Repository
{
    public class JointRecipeContext : DataConnection
    {
        public JointRecipeContext(string connectionString) : base(ProviderName.SQLite, connectionString) 
        {
            
        }

        public ITable<JointRecipeTable> Recipes => this.GetTable<JointRecipeTable>();
    }
}
