using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using System;
using System.Threading.Tasks;

namespace PNTZ.Mufta.Showcase.Data
{
    /// <summary>
    /// Загрузчик рецептов для реальных данных из базы данных
    /// </summary>
    public class RealRecipeLoader : IRecipeTableLoader
    {
        public JointRecipeTable LoadedRecipe { get; private set; }

        public event EventHandler<JointRecipeTable> RecipeLoaded;
        public event EventHandler<JointRecipeTable> RecipeLoadFailed;

        /// <summary>
        /// Загрузить рецепт из JointResult
        /// </summary>
        /// <param name="recipe">Рецепт из результата</param>
        public void LoadRecipe(JointRecipeTable recipe)
        {
            if (recipe == null)
            {
                RecipeLoadFailed?.Invoke(this, null);
                return;
            }

            LoadedRecipe = recipe;
            RecipeLoaded?.Invoke(this, recipe);
        }

        public Task LoadRecipeAsync(JointRecipeTable recipe)
        {
            throw new NotImplementedException();
        }
    }
}
