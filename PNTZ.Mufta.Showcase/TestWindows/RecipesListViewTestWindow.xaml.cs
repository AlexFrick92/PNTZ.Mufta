using PNTZ.Mufta.Showcase.Helper;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.ViewModel.Recipe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования контрола RecipesListView
    /// </summary>
    public partial class RecipesListViewTestWindow : Window
    {
        private RecipesListViewModel _viewModel;
        private RevertableJointRecipe _recipeA;
        private RevertableJointRecipe _recipeB;

        public RecipesListViewTestWindow()
        {
            InitializeComponent();
            InitializeViewModel();
            UpdateStatus("Контрол загружен и готов к работе.");
        }

        /// <summary>
        /// Инициализация ViewModel
        /// </summary>
        private void InitializeViewModel()
        {
            _viewModel = new RecipesListViewModel(new ObservableCollection<RevertableJointRecipe>());
            RecipesListView.DataContext = _viewModel;

            // Подписка на событие изменения выбранного рецепта
            _viewModel.SelectedRecipeChanged += ViewModel_SelectedRecipeChanged;
        }

        /// <summary>
        /// Обработчик события изменения выбранного рецепта
        /// </summary>
        private void ViewModel_SelectedRecipeChanged(object sender, RevertableJointRecipe recipe)
        {
            UpdateSelectedRecipeName(recipe.OriginalRecipe);
        }

        /// <summary>
        /// Обработчик кнопки загрузки рецептов
        /// </summary>
        private void LoadRecipesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создаём список тестовых рецептов
                var testRecipes = new List<JointRecipeTable>
                {
                    RecipeHelper.CreateTestRecipeLength(),
                    RecipeHelper.CreateTestRecipeTorque(),
                    RecipeHelper.CreateTestRecipeTorqueLength(),
                    RecipeHelper.CreateTestRecipeTorqueShoulder()
                };

                // Загружаем рецепты в ViewModel
                _viewModel.LoadRecipeList(testRecipes.Select(r => new RevertableJointRecipe(r)));

                UpdateStatus($"Загружено {testRecipes.Count} тестовых рецептов.");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка при загрузке рецептов: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновляет отображение выбранного рецепта
        /// </summary>
        private void UpdateSelectedRecipeName(JointRecipeTable recipe)
        {
            if (recipe != null)
            {
                SelectedRecipeNameText.Text = recipe.Name ?? "без имени";
                UpdateStatus($"Выбран рецепт: {recipe.Name}");
            }
            else
            {
                SelectedRecipeNameText.Text = "не выбран";
                UpdateStatus("Рецепт не выбран");
            }
        }

        /// <summary>
        /// Обновляет статусную строку
        /// </summary>
        private void UpdateStatus(string message)
        {
            StatusText.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        }

        /// <summary>
        /// Обработчик кнопки добавления/удаления рецепта А
        /// </summary>
        private void ToggleRecipeAButton_Click(object sender, RoutedEventArgs e)
        {
            if (_recipeA == null || !_viewModel.JointRecipes.Contains(_recipeA))
            {
                // Создаём рецепт А на основе Length рецепта
                var  tmprec = RecipeHelper.CreateTestRecipeLength();
                tmprec.Name = "RECIPE_A";
                tmprec.PIPE_TYPE = "PIPE_TYPE_A";

                _recipeA = new RevertableJointRecipe(tmprec);


                _viewModel.AddRecipe(_recipeA);
                ToggleRecipeAButton.Content = "Удалить рецепт А";
                ToggleRecipeAButton.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E74C3C"));
                UpdateStatus($"Рецепт А добавлен. Всего рецептов: {_viewModel.JointRecipes.Count}");
            }
            else
            {
                _viewModel.RemoveRecipe(_recipeA);
                ToggleRecipeAButton.Content = "Добавить рецепт А";
                ToggleRecipeAButton.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#27AE60"));
                UpdateStatus($"Рецепт А удалён. Всего рецептов: {_viewModel.JointRecipes.Count}");
            }
        }

        /// <summary>
        /// Обработчик кнопки добавления/удаления рецепта Б
        /// </summary>
        private void ToggleRecipeBButton_Click(object sender, RoutedEventArgs e)
        {
            if (_recipeB == null || !_viewModel.JointRecipes.Contains(_recipeB))
            {
                // Создаём рецепт Б на основе Torque рецепта
                var tmprec = RecipeHelper.CreateTestRecipeTorque();
                tmprec.Name = "RECIPE_B";
                tmprec.PIPE_TYPE = "PIPE_TYPE_B";

                _recipeB = new RevertableJointRecipe(tmprec);

                _viewModel.AddRecipe(_recipeB);
                ToggleRecipeBButton.Content = "Удалить рецепт Б";
                ToggleRecipeBButton.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#C0392B"));
                UpdateStatus($"Рецепт Б добавлен. Всего рецептов: {_viewModel.JointRecipes.Count}");
            }
            else
            {
                _viewModel.RemoveRecipe(_recipeB);
                ToggleRecipeBButton.Content = "Добавить рецепт Б";
                ToggleRecipeBButton.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E67E22"));
                UpdateStatus($"Рецепт Б удалён. Всего рецептов: {_viewModel.JointRecipes.Count}");
            }
        }

        /// <summary>
        /// Обработчик кнопки загрузки рецепта А
        /// </summary>
        private void LoadRecipeAButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаём рецепт А, если его ещё нет
            if (_recipeA == null)
            {
                var tmprec = RecipeHelper.CreateTestRecipeLength();
                tmprec.Name = "RECIPE_A";
                tmprec.PIPE_TYPE = "PIPE_TYPE_A";

                _recipeA = new RevertableJointRecipe(tmprec);
            }

            bool wasInList = _viewModel.JointRecipes.Contains(_recipeA);

            // Устанавливаем загруженный рецепт (поднимаем вверх или добавляем)
            _viewModel.SetLoadedRecipe(_recipeA);

            if (wasInList)
            {
                UpdateStatus($"Рецепт А поднят в начало списка. Всего рецептов: {_viewModel.JointRecipes.Count}");
            }
            else
            {
                UpdateStatus($"Рецепт А добавлен в начало списка. Всего рецептов: {_viewModel.JointRecipes.Count}");

                // Обновляем состояние кнопки toggle
                ToggleRecipeAButton.Content = "Удалить рецепт А";
                ToggleRecipeAButton.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E74C3C"));
            }
        }
    }
}
