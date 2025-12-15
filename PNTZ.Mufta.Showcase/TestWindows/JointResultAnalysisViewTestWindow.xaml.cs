using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.ViewModel.Joint;
using PNTZ.Mufta.Showcase.Data;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования контрола JointResultAnalysisView
    /// </summary>
    public partial class JointResultAnalysisViewTestWindow : Window
    {
        private JointResultAnalysisViewModel _viewModel;
        private TestResultsRepository _resultsRepository;
        private List<JointResultTable> _loadedResults;

        public JointResultAnalysisViewTestWindow()
        {
            InitializeComponent();
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            _viewModel = new JointResultAnalysisViewModel();
            AnalysisView.DataContext = _viewModel;
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
                // Открываем диалог выбора файла базы данных
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Выберите файл базы данных ResultsData.db",
                    Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*",
                    CheckFileExists = true
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string dbPath = openFileDialog.FileName;
                    UpdateStatus($"Загрузка базы данных: {dbPath}");

                    // Создаем репозиторий и загружаем результаты
                    _resultsRepository = new TestResultsRepository(dbPath);
                    _loadedResults = _resultsRepository.GetResults();

                    // Заполняем ListBox
                    ResultsListBox.ItemsSource = _loadedResults;

                    // Обновляем статус
                    DatabaseStatusText.Text = $"Загружено {_loadedResults.Count} записей из БД";
                    UpdateStatus($"База данных загружена: {_loadedResults.Count} записей");
                }
            }
            catch (Exception ex)
            {
                DatabaseStatusText.Text = $"Ошибка: {ex.Message}";
                UpdateStatus($"Ошибка загрузки БД: {ex.Message}");
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
                    JointResult realResult = selectedTable.ToJointResult();

                    // Передаем результат в ViewModel контрола
                    _viewModel.CurrentResult = realResult;

                    UpdateStatus($"Результат загружен: {realResult.Recipe.Name}, {realResult.Series.Count} точек");
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Ошибка загрузки данных: {ex.Message}");
                }
            }
        }
    }
}
