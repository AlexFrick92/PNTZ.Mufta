using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.ViewModel.Recipe
{
    public class RecipesViewModel : BaseViewModel
    {
        string nameFilter = string.Empty;
        public RecipesViewModel(LocalRepository repository)
        {
            var filtered = repository.GetRecipes(r =>
                string.IsNullOrEmpty(nameFilter)
                || r.Name.Contains(nameFilter)
                || r.TimeStamp.ToString().Contains(nameFilter)).Select(r => r.ToJointRecipe());

            RecipesList.SelectedRecipeChanged += (o, r) => EditRecipeViewModel.SetEditingRecipe(r);
            
            RecipesList.LoadRecipeList(filtered);

        }

        /// <summary>
        /// Редактирование рецепта
        /// </summary>
        public EditRecipeViewModel EditRecipeViewModel { get; set; } = new EditRecipeViewModel();
        /// <summary>
        /// Список рецептов
        /// </summary>
        public RecipesListViewModel RecipesList { get; set; } = new RecipesListViewModel();

    }
}
