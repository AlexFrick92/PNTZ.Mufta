using PNTZ.Mufta.TPCApp.DpConnect;
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
        private readonly RecipeDpWorker _recipeDpWorker;
        public ActualRecipe(RecipeDpWorker recipeDpWorker)
        {
            _recipeDpWorker = recipeDpWorker;
        }
        public JointRecipeTable LoadedRecipe => throw new NotImplementedException();

        public event EventHandler<JointRecipeTable> RecipeLoaded;
        public event EventHandler<JointRecipeTable> RecipeLoadFailed;

        public async Task LoadRecipeAsync(JointRecipeTable recipe)
        {            
            await _recipeDpWorker.LoadRecipeAsync(recipe);            
            RecipeLoaded?.Invoke(this, recipe); 
        }
    }
}
