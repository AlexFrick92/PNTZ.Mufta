


using PNTZ.Mufta.TPCApp.Domain;

using Promatis.Core.Logging;

using System.Collections.Generic;

using static PNTZ.Mufta.TPCApp.App;

namespace PNTZ.Mufta.TPCApp.Repository
{
    public class RepositoryContext
    {
        ILogger logger;

        string StoragePath = App.AppInstance.CurrentDirectory + "/Repository/processdata.db";

        public RepositoryContext(ILogger logger)
        {
            this.logger = logger;

            //using (var connection = new SqliteConnection($"Data Source={StoragePath}"))
            //{
            //    connection.Open();
            //}
        }


        public void SaveRecipes(IEnumerable<JointRecipe> jointRecipes)
        {
            logger.Info("Рецепты сохранены.");
        }
        public IEnumerable<JointRecipe> LoadRecipes()
        {
            logger.Info("Загружаем рецепты...");
            logger.Info("Рецепты загружены.");
            return new List<JointRecipe>() { new JointRecipe() { Name = "HSP-123" }, new JointRecipe() { Name = "DPV-123-5423"} };
        }
    }
}
