using PNTZ.Mufta.Showcase.Helper;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using System;
using System.Threading.Tasks;

namespace PNTZ.Mufta.Showcase.Data
{
    /// <summary>
    /// Мок для IRecipeLoader, симулирующий асинхронную загрузку рецептов
    /// </summary>
    public class MockRecipeLoader : IRecipeTableLoader
    {
        public JointRecipeTable LoadedRecipe { get; private set; }

        public event EventHandler<JointRecipeTable> RecipeLoaded;
        public event EventHandler<JointRecipeTable> RecipeLoadFailed;

        public async Task LoadRecipeAsync(JointRecipeTable recipe)
        {
            await Task.Run(async () =>
            {
                await Task.Delay(3000);
                LoadedRecipe = recipe;
                RecipeLoaded?.Invoke(this, recipe);
            });
        }

        /// <summary>
        /// Загрузить тестовый рецепт по длине
        /// </summary>
        public void LoadRecipeLength()
        {
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                var recipe = RecipeHelper.CreateTestRecipeLength();
                LoadedRecipe = recipe;
                RecipeLoaded?.Invoke(this, recipe);
            });
        }

        /// <summary>
        /// Загрузить тестовый рецепт по моменту
        /// </summary>
        public void LoadRecipeTorque()
        {
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                var recipe = RecipeHelper.CreateTestRecipeTorque();
                LoadedRecipe = recipe;
                RecipeLoaded?.Invoke(this, recipe);
            });
        }

        /// <summary>
        /// Загрузить тестовый рецепт по моменту и длине
        /// </summary>
        public void LoadRecipeTorqueLength()
        {
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                var recipe = RecipeHelper.CreateTestRecipeTorqueLength();
                LoadedRecipe = recipe;
                RecipeLoaded?.Invoke(this, recipe);
            });
        }

        /// <summary>
        /// Загрузить тестовый рецепт по моменту до упора
        /// </summary>
        public void LoadRecipeTorqueShoulder()
        {
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                var recipe = RecipeHelper.CreateTestRecipeTorqueShoulder();
                LoadedRecipe = recipe;
                RecipeLoaded?.Invoke(this, recipe);
            });
        }
    }
}
