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
        private JointRecipe _selectedRecipe;

        /// <summary>
        /// Список рецептов
        /// </summary>
        public ObservableCollection<JointRecipe> JointRecipes { get; private set; } = new ObservableCollection<JointRecipe>();

        /// <summary>
        /// Выбранный рецепт
        /// </summary>
        public JointRecipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                if (_selectedRecipe != value)
                {
                    _selectedRecipe = value;
                    OnPropertyChanged(nameof(SelectedRecipe));
                }
            }
        }

        public void LoadRecipeList(IEnumerable<JointRecipe> recipes)
        {
            JointRecipes.Clear();
            foreach (var recipe in recipes)
            {
                JointRecipes.Add(recipe);
            }
        }
    }
}
