using PNTZ.Mufta.TPCApp.ViewModel.Recipe;
using System;
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
