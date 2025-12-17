using System;
using System.Windows;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования контрола RecipesView
    /// </summary>
    public partial class RecipesViewTestWindow : Window
    {
        public RecipesViewTestWindow()
        {
            InitializeComponent();
            UpdateStatus("Контрол загружен и готов к работе.");
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
