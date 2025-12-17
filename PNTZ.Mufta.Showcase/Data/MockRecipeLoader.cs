using PNTZ.Mufta.Showcase.Helper;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Threading.Tasks;

namespace PNTZ.Mufta.Showcase.Data
{
    /// <summary>
    /// Мок для IRecipeLoader, симулирующий асинхронную загрузку рецептов
    /// </summary>
    public class MockRecipeLoader : IRecipeLoader
    {
        public JointRecipe LoadedRecipe { get; private set; }

        public event EventHandler<JointRecipe> RecipeLoaded;
        public event EventHandler<JointRecipe> RecipeLoadFailed;

        public async Task LoadRecipeAsync(JointRecipe recipe)
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
