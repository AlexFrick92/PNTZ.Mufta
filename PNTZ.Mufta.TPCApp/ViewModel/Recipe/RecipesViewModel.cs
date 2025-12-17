using Desktop.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.ViewModel.Recipe
{
    public class RecipesViewModel : BaseViewModel
    {
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
