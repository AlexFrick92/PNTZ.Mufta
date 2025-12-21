using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
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
        private RevertableJointRecipe _selectedRecipe;
        private RevertableJointRecipe _loadedRecipe;

        /// <summary>
        /// Конструктор принимает коллекцию рецептов извне
        /// </summary>
        /// <param name="recipes">Коллекция рецептов для работы</param>
        public RecipesListViewModel(ObservableCollection<RevertableJointRecipe> recipes)
        {
            JointRecipes = recipes ?? throw new ArgumentNullException(nameof(recipes));
        }

        /// <summary>
        /// Событие изменения выбранного рецепта
        /// </summary>
        public event EventHandler<RevertableJointRecipe> SelectedRecipeChanged;

        /// <summary>
        /// Список рецептов
        /// </summary>
        public ObservableCollection<RevertableJointRecipe> JointRecipes { get; }

        /// <summary>
        /// Выбранный рецепт
        /// </summary>
        public RevertableJointRecipe SelectedRecipe
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
        /// Загруженный рецепт (для визуального выделения)
        /// </summary>
        public RevertableJointRecipe LoadedRecipe
        {
            get => _loadedRecipe;
            set
            {
                if (_loadedRecipe != value)
                {
                    _loadedRecipe = value;
                    OnPropertyChanged(nameof(LoadedRecipe));
                }
            }
        }
        /// <summary>
        /// Загрузить новый список рецептов
        /// </summary>
        /// <param name="recipes"></param>
        public void LoadRecipeList(IEnumerable<RevertableJointRecipe> recipes)
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
        public void AddRecipe(RevertableJointRecipe recipe)
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
        public void RemoveRecipe(RevertableJointRecipe recipe)
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
        public void SetLoadedRecipe(RevertableJointRecipe recipe)
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

            // Устанавливаем как загруженный рецепт
            LoadedRecipe = recipe;
        }

        /// <summary>
        /// Переместить рецепт на первую позицию в списке
        /// </summary>
        /// <param name="recipe">Рецепт для перемещения</param>
        public void MoveToTop(RevertableJointRecipe recipe)
        {
            if (recipe == null)
                return;

            int index = JointRecipes.IndexOf(recipe);

            if (index > 0)
            {
                // Рецепт найден и не на первой позиции - перемещаем наверх
                JointRecipes.Move(index, 0);
            }
        }
    }
}
