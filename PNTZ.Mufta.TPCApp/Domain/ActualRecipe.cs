using PNTZ.Mufta.TPCApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Domain
{
    /// <summary>
    /// Работа с актуальным рецептом
    /// Загрузка или выгрузка рецепта
    /// </summary>
    public class ActualRecipe : IRecipeTableLoader
    {
        public JointRecipeTable LoadedRecipe => throw new NotImplementedException();

        public event EventHandler<JointRecipeTable> RecipeLoaded;
        public event EventHandler<JointRecipeTable> RecipeLoadFailed;

        public Task LoadRecipeAsync(JointRecipeTable recipe)
        {
            throw new NotImplementedException();
        }
    }
}
