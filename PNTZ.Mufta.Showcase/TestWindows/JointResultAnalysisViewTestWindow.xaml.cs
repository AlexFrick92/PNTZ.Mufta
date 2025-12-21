using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.ViewModel.Joint;
using PNTZ.Mufta.Showcase.Data;
using Promatis.Core.Logging;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования контрола JointResultAnalysisView
    /// </summary>
    public partial class JointResultAnalysisViewTestWindow : Window
    {
        private JointResultAnalysisViewModel _viewModel;
        private LocalRepository _repository;
        private List<JointResultTable> _loadedResults;

        public JointResultAnalysisViewTestWindow()
        {
            InitializeComponent();
            InitializeViewModel();
            InitializeRepository();
        }

        private void InitializeViewModel()
        {
            _viewModel = new JointResultAnalysisViewModel();
            AnalysisView.DataContext = _viewModel;
        }

        private void InitializeRepository()
        {
            _repository = new LocalRepository(new ConsoleLogger());
        }

        /// <summary>
        /// Обновляет статусную строку
        /// </summary>
        private void UpdateStatus(string message)
        {
            StatusText.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        }

        /// <summary>
        /// Обработчик кнопки "Загрузить базу данных"
        /// </summary>
        private void BtnLoadDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatus("Загрузка результатов из локального репозитория...");

                // Загружаем результаты через LocalRepository
                _loadedResults = _repository.GetResults();

                // Заполняем ListBox
                ResultsListBox.ItemsSource = _loadedResults;

                // Обновляем статус
                DatabaseStatusText.Text = $"Загружено {_loadedResults.Count} записей из БД";
                UpdateStatus($"Результаты загружены: {_loadedResults.Count} записей");
            }
            catch (Exception ex)
            {
                DatabaseStatusText.Text = $"Ошибка: {ex.Message}";
                UpdateStatus($"Ошибка загрузки: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработчик выбора записи из списка результатов
        /// </summary>
        private void ResultsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ResultsListBox.SelectedItem is JointResultTable selectedTable)
            {
                try
                {
                    // Десериализуем выбранную запись в JointResult
                    JointResultTable realResult = selectedTable;

                    // Передаем результат в ViewModel контрола
                    _viewModel.CurrentResult = realResult;

                    UpdateStatus($"Результат загружен: {realResult.Recipe.Name}, {realResult.PointSeries.Count} точек");
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Ошибка загрузки данных: {ex.Message}");
                }
            }
        }
    }
}
