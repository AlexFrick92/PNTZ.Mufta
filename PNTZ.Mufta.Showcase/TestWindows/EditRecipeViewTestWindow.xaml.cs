using System;
using System.Windows;
using PNTZ.Mufta.TPCApp.ViewModel.Recipe;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Domain.Helpers;
using PNTZ.Mufta.Showcase.Helper;
using PNTZ.Mufta.Showcase.Data;
using PNTZ.Mufta.TPCApp.Repository;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования контрола EditRecipeView
    /// </summary>
    public partial class EditRecipeViewTestWindow : Window
    {
        private EditRecipeViewModel _viewModel;
        private System.Collections.Generic.Dictionary<string, JointRecipeTable> _savedRecipes;

        public EditRecipeViewTestWindow()
        {
            InitializeComponent();
            _savedRecipes = new System.Collections.Generic.Dictionary<string, JointRecipeTable>();
            InitializeViewModel();
            UpdateStatus("Контрол загружен и готов к работе.");

            // Подписываемся на событие закрытия окна для очистки
            Closed += OnWindowClosed;
        }

        private void InitializeViewModel()
        {
            _viewModel = new EditRecipeViewModel(new MockRecipeLoader());
            _viewModel.RecipeSaved += OnRecipeSaved;
            _viewModel.RecipeCancelled += OnRecipeCancelled;
            EditRecipeView.DataContext = _viewModel;
        }

        private void OnRecipeSaved(object sender, JointRecipeTable recipe)
        {
            // Рецепт уже обновлён в памяти (это оригинал из словаря)
            UpdateStatus($"✅ Рецепт сохранён: {recipe.Name} (ID: {recipe.Id})");
            MessageBox.Show(
                $"Рецепт успешно сохранён!\n\nНазвание: {recipe.Name}\nРежим: {recipe.JointMode}\n\nИзменения применены к оригинальному рецепту.",
                "Сохранение рецепта",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void OnRecipeCancelled(object sender, EventArgs e)
        {
            UpdateStatus("↩️ Изменения отменены, восстановлена оригинальная версия рецепта");
            MessageBox.Show(
                "Все изменения отменены.\n\nРецепт восстановлен до состояния последнего сохранения.",
                "Отмена изменений",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.RecipeSaved -= OnRecipeSaved;
                _viewModel.RecipeCancelled -= OnRecipeCancelled;
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
        /// Генерирует ключ для хранения рецепта в словаре
        /// </summary>
        private string GetRecipeKey(JointMode mode)
        {
            return $"Test_{mode}";
        }

        /// <summary>
        /// Загружает рецепт с проверкой сохранённых данных
        /// </summary>
        private void LoadRecipe(JointMode mode, Func<JointRecipeTable> createDefaultRecipe)
        {
            var recipeKey = GetRecipeKey(mode);
            JointRecipeTable recipe;

            if (_savedRecipes.ContainsKey(recipeKey))
            {
                // Загружаем сохранённую версию (оригинал)
                // EditRecipeViewModel создаст копию для редактирования
                recipe = _savedRecipes[recipeKey];
                UpdateStatus($"📂 Рецепт загружен из памяти: {recipe.Name} (Режим: {recipe.JointMode})");
            }
            else
            {
                // Создаём новый тестовый рецепт и сохраняем в словарь
                recipe = createDefaultRecipe();
                _savedRecipes[recipeKey] = recipe;
                UpdateStatus($"🆕 Создан новый рецепт: {recipe.Name} (Режим: {recipe.JointMode})");
            }

            // Передаём оригинал в ViewModel, он сам создаст копию для редактирования
            _viewModel.SetEditingRecipe(recipe);
        }

        private void BtnLoadRecipeLength_Click(object sender, RoutedEventArgs e)
        {
            LoadRecipe(JointMode.Length, RecipeHelper.CreateTestRecipeLength);
        }

        private void BtnLoadRecipeTorque_Click(object sender, RoutedEventArgs e)
        {
            LoadRecipe(JointMode.Torque, RecipeHelper.CreateTestRecipeTorque);
        }

        private void BtnLoadRecipeTorqueLength_Click(object sender, RoutedEventArgs e)
        {
            LoadRecipe(JointMode.TorqueLength, RecipeHelper.CreateTestRecipeTorqueLength);
        }

        private void BtnLoadRecipeTorqueShoulder_Click(object sender, RoutedEventArgs e)
        {
            LoadRecipe(JointMode.TorqueShoulder, RecipeHelper.CreateTestRecipeTorqueShoulder);
        }
    }
}
