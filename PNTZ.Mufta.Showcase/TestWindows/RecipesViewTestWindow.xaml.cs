using PNTZ.Mufta.Showcase.Data;
using PNTZ.Mufta.TPCApp.ViewModel.Recipe;
using Promatis.Core.Logging;
using System;
using System.Windows;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования контрола RecipesView
    /// </summary>
    public partial class RecipesViewTestWindow : Window
    {
        private RecipesViewModel _viewModel;
        public RecipesViewTestWindow()
        {
            InitializeComponent();
            InitializeViewModel();
            UpdateStatus("Контрол загружен и готов к работе.");
        }

        /// <summary>
        /// Обновляет статусную строку
        /// </summary>
        private void UpdateStatus(string message)
        {
            StatusText.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        }
        private void InitializeViewModel()
        {
            _viewModel = new RecipesViewModel(new TPCApp.Repository.LocalRepository(new ConsoleLogger()), new MockRecipeLoader());    
            RecipesView.DataContext = _viewModel;
        }
    }
}
