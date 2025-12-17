using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.ViewModel.Recipe
{
    /// <summary>
    /// Модель представления списка рецептов.
    /// </summary>
    public class RecipesListViewModel : BaseViewModel
    {
        /// <summary>
        /// Список рецептов
        /// </summary>
        public ObservableCollection<JointRecipeViewModel> JointRecipes { get; private set; } = new ObservableCollection<JointRecipeViewModel>();

        public void LoadRecipeList(IEnumerable<JointRecipe> recipes)
        {
            JointRecipes.Clear();
            foreach (var recipe in recipes)
            {
                JointRecipes.Add(new JointRecipeViewModel(recipe));
            }
        }
        
        public JointRecipe SelectedRecipe { get; set; }
        
    }
}
