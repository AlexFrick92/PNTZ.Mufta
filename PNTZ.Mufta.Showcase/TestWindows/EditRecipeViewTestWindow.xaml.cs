using System;
using System.Windows;
using PNTZ.Mufta.TPCApp.ViewModel.Recipe;

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
        }

        private void InitializeViewModel()
        {
            _viewModel = new EditRecipeViewModel();
            EditRecipeView.DataContext = _viewModel;
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
