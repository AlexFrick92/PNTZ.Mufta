using PNTZ.Mufta.Showcase.Helper;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.ViewModel.Recipe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования контрола RecipesListView
    /// </summary>
    public partial class RecipesListViewTestWindow : Window
    {
        private RecipesListViewModel _viewModel;

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
            _viewModel = new RecipesListViewModel();
            RecipesListView.DataContext = _viewModel;

            // Подписка на изменение свойств ViewModel
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        /// <summary>
        /// Обработчик изменения свойств ViewModel
        /// </summary>
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RecipesListViewModel.SelectedRecipe))
            {
                UpdateSelectedRecipeName();
            }
        }

        /// <summary>
        /// Обработчик кнопки загрузки рецептов
        /// </summary>
        private void LoadRecipesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создаём список тестовых рецептов
                var testRecipes = new List<JointRecipe>
                {
                    RecipeHelper.CreateTestRecipeLength(),
                    RecipeHelper.CreateTestRecipeTorque(),
                    RecipeHelper.CreateTestRecipeTorqueLength(),
                    RecipeHelper.CreateTestRecipeTorqueShoulder()
                };

                // Загружаем рецепты в ViewModel
                _viewModel.LoadRecipeList(testRecipes);

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
        private void UpdateSelectedRecipeName()
        {
            if (_viewModel.SelectedRecipe != null)
            {
                SelectedRecipeNameText.Text = _viewModel.SelectedRecipe.Name ?? "без имени";
                UpdateStatus($"Выбран рецепт: {_viewModel.SelectedRecipe.Name}");
            }
            else
            {
                SelectedRecipeNameText.Text = "не выбран";
            }
        }

        /// <summary>
        /// Обновляет статусную строку
        /// </summary>
        private void UpdateStatus(string message)
        {
            StatusText.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        }
    }
}
