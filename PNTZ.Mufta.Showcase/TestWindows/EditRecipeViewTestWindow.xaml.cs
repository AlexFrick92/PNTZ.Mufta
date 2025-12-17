using System;
using System.Windows;
using PNTZ.Mufta.TPCApp.ViewModel.Recipe;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.Showcase.Helper;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования контрола EditRecipeView
    /// </summary>
    public partial class EditRecipeViewTestWindow : Window
    {
        private EditRecipeViewModel _viewModel;

        public EditRecipeViewTestWindow()
        {
            InitializeComponent();
            InitializeViewModel();
            UpdateStatus("Контрол загружен и готов к работе.");

            // Подписываемся на событие закрытия окна для очистки
            Closed += OnWindowClosed;
        }

        private void InitializeViewModel()
        {
            _viewModel = new EditRecipeViewModel();
            _viewModel.RecipeSaved += OnRecipeSaved;
            EditRecipeView.DataContext = _viewModel;
        }

        private void OnRecipeSaved(object sender, JointRecipe recipe)
        {
            UpdateStatus($"✅ Рецепт сохранён: {recipe.Name} (ID: {recipe.Id})");
            MessageBox.Show(
                $"Рецепт успешно сохранён!\n\nНазвание: {recipe.Name}\nРежим: {recipe.JointMode}",
                "Сохранение рецепта",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.RecipeSaved -= OnRecipeSaved;
            }
        }

        /// <summary>
        /// Обновляет статусную строку
        /// </summary>
        private void UpdateStatus(string message)
        {
            StatusText.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        }

        private void BtnLoadRecipeLength_Click(object sender, RoutedEventArgs e)
        {
            var recipe = RecipeHelper.CreateTestRecipeLength();
            _viewModel.SetEditingRecipe(recipe);
            UpdateStatus($"Рецепт загружен: {recipe.Name} (Режим: {recipe.JointMode})");
        }

        private void BtnLoadRecipeTorque_Click(object sender, RoutedEventArgs e)
        {
            var recipe = RecipeHelper.CreateTestRecipeTorque();
            _viewModel.SetEditingRecipe(recipe);
            UpdateStatus($"Рецепт загружен: {recipe.Name} (Режим: {recipe.JointMode})");
        }

        private void BtnLoadRecipeTorqueLength_Click(object sender, RoutedEventArgs e)
        {
            var recipe = RecipeHelper.CreateTestRecipeTorqueLength();
            _viewModel.SetEditingRecipe(recipe);
            UpdateStatus($"Рецепт загружен: {recipe.Name} (Режим: {recipe.JointMode})");
        }

        private void BtnLoadRecipeTorqueShoulder_Click(object sender, RoutedEventArgs e)
        {
            var recipe = RecipeHelper.CreateTestRecipeTorqueShoulder();
            _viewModel.SetEditingRecipe(recipe);
            UpdateStatus($"Рецепт загружен: {recipe.Name} (Режим: {recipe.JointMode})");
        }
    }
}
