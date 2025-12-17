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
        /// Событие изменения выбранного рецепта
        /// </summary>
        public event EventHandler<JointRecipe> SelectedRecipeChanged;

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
                    SelectedRecipeChanged?.Invoke(this, value);
                }
            }
        }
        /// <summary>
        /// Загрузить новый список рецептов
        /// </summary>
        /// <param name="recipes"></param>
        public void LoadRecipeList(IEnumerable<JointRecipe> recipes)
        {
            JointRecipes.Clear();
            foreach (var recipe in recipes)
            {
                JointRecipes.Add(recipe);
            }
        }

        /// <summary>
        /// Добавить рецепт в коллекцию
        /// </summary>
        /// <param name="recipe"></param>
        public void AddRecipe(JointRecipe recipe)
        {
            if (recipe != null)
            {
                JointRecipes.Add(recipe);
            }
        }

        /// <summary>
        /// Удалить рецепт из коллекции
        /// </summary>
        /// <param name="recipe"></param>
        public void RemoveRecipe(JointRecipe recipe)
        {
            if (recipe != null)
            {
                JointRecipes.Remove(recipe);
            }
        }

        /// <summary>
        /// Установить загруженный рецепт (переместить в начало списка или добавить)
        /// </summary>
        /// <param name="recipe"></param>
        public void SetLoadedRecipe(JointRecipe recipe)
        {
            if (recipe == null)
                return;

            int index = JointRecipes.IndexOf(recipe);

            if (index >= 0)
            {
                // Рецепт уже в списке - перемещаем на первую позицию
                if (index != 0)
                {
                    JointRecipes.Move(index, 0);
                }
            }
            else
            {
                // Рецепта нет в списке - добавляем в начало
                JointRecipes.Insert(0, recipe);
            }
        }
    }
}
