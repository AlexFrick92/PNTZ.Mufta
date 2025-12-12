using PNTZ.Mufta.TPCApp.Domain;
using System;

namespace PNTZ.Mufta.Showcase.Data
{
    /// <summary>
    /// Загрузчик рецептов для реальных данных из базы данных
    /// </summary>
    public class RealRecipeLoader : IRecipeLoader
    {
        public JointRecipe LoadedRecipe { get; private set; }

        public event EventHandler<JointRecipe> RecipeLoaded;
        public event EventHandler<JointRecipe> RecipeLoadFailed;

        /// <summary>
        /// Загрузить рецепт из JointResult
        /// </summary>
        /// <param name="recipe">Рецепт из результата</param>
        public void LoadRecipe(JointRecipe recipe)
        {
            if (recipe == null)
            {
                RecipeLoadFailed?.Invoke(this, null);
                return;
            }

            LoadedRecipe = recipe;
            RecipeLoaded?.Invoke(this, recipe);
        }
    }
}
