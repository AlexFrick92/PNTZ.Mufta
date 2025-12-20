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
    public class RecipesViewModel : BaseViewModel
    {
        string nameFilter = string.Empty;
        private IRecipeTableLoader _loader;
        private ObservableCollection<JointRecipeTable> _recipes = new ObservableCollection<JointRecipeTable>();

        public RecipesViewModel(LocalRepository repository, IRecipeTableLoader loader)
        {
            _loader = loader;

            // Создаём RecipesList с нашей коллекцией
            RecipesList = new RecipesListViewModel(_recipes);
            EditRecipeViewModel = new EditRecipeViewModel(_loader);

            _loader.RecipeLoaded += (s, e) =>
            {
                RecipesList.LoadedRecipe = e;
                EditRecipeViewModel.SetLoadedRecipe(e);
            };

            var filtered = repository.GetRecipes(r =>
                string.IsNullOrEmpty(nameFilter)
                || r.Name.Contains(nameFilter)
                || r.TimeStamp.ToString().Contains(nameFilter)).OrderByDescending(r => r.TimeStamp);

            // Подписываемся на события списка рецептов
            RecipesList.SelectedRecipeChanged += OnSelectedRecipeChanged;

            // Подписываемся на события редактирования рецепта
            EditRecipeViewModel.RecipeSaved += OnRecipeSaved;
            EditRecipeViewModel.RecipeCancelled += OnRecipeCancelled;
            EditRecipeViewModel.RecipeDeleted += OnRecipeDeleted;

            // Загружаем рецепты в коллекцию
            foreach (var recipe in filtered)
            {
                _recipes.Add(recipe);
            }
        }

        /// <summary>
        /// Редактирование рецепта
        /// </summary>
        public EditRecipeViewModel EditRecipeViewModel { get; set; }
        /// <summary>
        /// Список рецептов
        /// </summary>
        public RecipesListViewModel RecipesList { get; set; }

        /// <summary>
        /// Обработчик выбора рецепта из списка
        /// </summary>
        private void OnSelectedRecipeChanged(object sender, JointRecipeTable recipe)
        {
            if (recipe != null)
            {
                EditRecipeViewModel.SetEditingRecipe(recipe);
            }
        }

        /// <summary>
        /// Обработчик сохранения рецепта
        /// </summary>
        private void OnRecipeSaved(object sender, JointRecipeTable savedRecipe)
        {
            // Обновляем временную метку в списке
            savedRecipe.TimeStamp = DateTime.UtcNow;

            // Перемещаем обновлённый рецепт на первую позицию
            RecipesList.MoveToTop(savedRecipe);
        }

        /// <summary>
        /// Обработчик отмены изменений
        /// </summary>
        private void OnRecipeCancelled(object sender, EventArgs e)
        {
            // Пока ничего не делаем
        }

        /// <summary>
        /// Обработчик удаления рецепта
        /// </summary>
        private void OnRecipeDeleted(object sender, JointRecipeTable deletedRecipe)
        {
            // Удаляем рецепт из списка
            RecipesList.RemoveRecipe(deletedRecipe);
        }

    }
}
