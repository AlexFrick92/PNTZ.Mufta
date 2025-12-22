using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.View.Recipe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PNTZ.Mufta.TPCApp.ViewModel.Recipe
{
    public class RecipesViewModel : BaseViewModel
    {
        string nameFilter = string.Empty;
        private IRecipeTableLoader _loader;
        private LocalRepository _repository;
        private ObservableCollection<RevertableJointRecipe> _recipes = new ObservableCollection<RevertableJointRecipe>();

        public RecipesViewModel(LocalRepository repository, IRecipeTableLoader loader)
        {
            _repository = repository;
            _loader = loader;

            // Создаём RecipesList с нашей коллекцией
            RecipesList = new RecipesListViewModel(_recipes);
            EditRecipeViewModel = new EditRecipeViewModel(_loader);

            _loader.RecipeLoaded += (s, e) =>
            {
                // Ищем RevertableJointRecipe, соответствующий загруженному рецепту
                var revertableRecipe = _recipes.FirstOrDefault(r => r.OriginalRecipe == e);
                if (revertableRecipe != null)
                {
                    RecipesList.LoadedRecipe = revertableRecipe;
                    EditRecipeViewModel.SetLoadedRecipe(revertableRecipe);
                }
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

            // Команда загрузки рецепта
            LoadRecipeCommand = new RelayCommand(LoadRecipe, CanLoadRecipe);

            // Команда создания нового рецепта
            NewRecipeCommand = new RelayCommand(CreateNewRecipe);

            // Загружаем рецепты в коллекцию, оборачивая каждый в RevertableJointRecipe
            foreach (var recipe in filtered)
            {
                _recipes.Add(new RevertableJointRecipe(recipe));
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
        /// Команда загрузки рецепта в PLC
        /// </summary>
        public ICommand LoadRecipeCommand { get; }

        /// <summary>
        /// Команда создания нового рецепта
        /// </summary>
        public ICommand NewRecipeCommand { get; }

        /// <summary>
        /// Флаг выполнения загрузки рецепта
        /// </summary>
        public bool RecipeLoadingInProgress { get; private set; } = false;

        private bool CanLoadRecipe(object parameter)
        {
            return EditRecipeViewModel.IsRecipeReadyForOperations && _loader != null && !RecipeLoadingInProgress;
        }

        private void LoadRecipe(object parameter)
        {
            try
            {
                RecipeLoadingInProgress = true;
                OnPropertyChanged(nameof(RecipeLoadingInProgress));

                // Получаем RevertableJointRecipe из списка
                var revertableRecipe = RecipesList.SelectedRecipe;
                if (revertableRecipe == null)
                    return;

                // Используем оригинальный рецепт для загрузки в PLC
                var recipeToLoad = revertableRecipe.OriginalRecipe;

                // Создаём ViewModel и окно загрузки
                var loadingViewModel = new LoadingRecipeViewModel(_loader, recipeToLoad);
                var loadingWindow = new LoadingRecipeView(loadingViewModel);
                loadingWindow.Owner = Application.Current.MainWindow;

                // Показываем модальное окно
                // Вся логика загрузки инкапсулирована внутри LoadingRecipeViewModel
                loadingWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                // Обработка критических ошибок создания окна
                MessageBox.Show($"Ошибка загрузки рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                RecipeLoadingInProgress = false;
                OnPropertyChanged(nameof(RecipeLoadingInProgress));
            }
        }

        /// <summary>
        /// Создание нового рецепта
        /// </summary>
        private void CreateNewRecipe(object parameter)
        {
            NewRecipeViewModel newRecipeViewModel = new NewRecipeViewModel();
            NewRecipeView newRecipeView = new NewRecipeView(newRecipeViewModel);
            newRecipeView.Owner = Application.Current.MainWindow;

            newRecipeViewModel.RecipeCreated += (o, createdRecipe) =>
            {
                // Оборачиваем в RevertableJointRecipe
                var revertableRecipe = new RevertableJointRecipe(createdRecipe);

                // Добавляем в начало списка
                _recipes.Insert(0, revertableRecipe);

                // Устанавливаем для редактирования
                EditRecipeViewModel.SetEditingRecipe(revertableRecipe);

                // Устанавливаем как выбранный в списке
                RecipesList.SelectedRecipe = revertableRecipe;

                newRecipeView.Close();
            };

            newRecipeViewModel.Canceled += (o, e) =>
            {
                newRecipeView.Close();
            };

            newRecipeView.ShowDialog();
        }

        /// <summary>
        /// Обработчик выбора рецепта из списка
        /// </summary>
        private void OnSelectedRecipeChanged(object sender, RevertableJointRecipe recipe)
        {
            if (recipe != null)
            {
                EditRecipeViewModel.SetEditingRecipe(recipe);
            }
        }

        /// <summary>
        /// Обработчик сохранения рецепта
        /// </summary>
        private async void OnRecipeSaved(object sender, RevertableJointRecipe savedRecipe)
        {
            try
            {
                // Сохраняем рецепт в базу данных асинхронно (метод сам обновит TimeStamp)
                await _repository.SaveRecipeAsync(savedRecipe.OriginalRecipe);

                // Перемещаем обновлённый рецепт на первую позицию
                RecipesList.MoveToTop(savedRecipe);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
        private void OnRecipeDeleted(object sender, RevertableJointRecipe deletedRecipe)
        {
            // Удаляем рецепт из списка
            RecipesList.RemoveRecipe(deletedRecipe);
        }

    }
}
