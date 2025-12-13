using System;
using System.Collections.Generic;
using System.Globalization;
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

                    UpdateStatus($"Загружены данные: {realResult.Recipe.Name}, {realResult.Series.Count} точек");
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Ошибка загрузки данных: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Обработчик изменения слайдера SearchStartRatio
        /// </summary>
        private void SliderSearchStartRatio_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Обновляем параметр в ViewModel, что автоматически обновит визуализацию зоны
            if (_viewModel != null)
            {
                _viewModel.SearchStartRatio = e.NewValue;
            }
        }

        /// <summary>
        /// Обработчик кнопки "Выполнить расчёт" детектора заплечника
        /// </summary>
        private void BtnRunDetection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, что результат загружен
                if (_viewModel.CurrentResult == null)
                {
                    DetectionResultText.Text = "⚠ Сначала выберите результат из списка";
                    UpdateStatus("Ошибка: результат не выбран");
                    return;
                }

                // Читаем параметры из UI
                if (!int.TryParse(TxtWindowSize.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int windowSize) || windowSize <= 0)
                {
                    DetectionResultText.Text = "⚠ Неверное значение WindowSize";
                    UpdateStatus("Ошибка: некорректный WindowSize");
                    return;
                }

                if (!double.TryParse(TxtSigmaMultiplier.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double sigmaMultiplier) || sigmaMultiplier <= 0)
                {
                    DetectionResultText.Text = "⚠ Неверное значение SigmaMultiplier";
                    UpdateStatus("Ошибка: некорректный SigmaMultiplier");
                    return;
                }

                double searchStartRatio = SliderSearchStartRatio.Value;

                // Передаём параметры в ViewModel
                _viewModel.WindowSize = windowSize;
                _viewModel.SigmaMultiplier = sigmaMultiplier;
                _viewModel.SearchStartRatio = searchStartRatio;

                UpdateStatus("Выполняется расчёт детектора заплечника...");
                DetectionResultText.Text = "⏳ Выполняется расчёт...";

                // Запускаем детектор
                _viewModel.RunShoulderDetection();

                // Получаем результат из последнего расчёта
                var result = _viewModel.GetLastDetectionResult();

                if (result?.ShoulderPointIndex.HasValue == true)
                {
                    int index = result.ShoulderPointIndex.Value;
                    var point = _viewModel.CurrentResult.Series[index];

                    DetectionResultText.Text = $"✓ Точка найдена!\n" +
                        $"Индекс: {index}\n" +
                        $"Момент: {point.Torque:F1} Nm\n" +
                        $"Обороты: {point.Turns:F3}\n" +
                        $"Время: {point.TimeStamp / 1000.0:F2} сек";

                    UpdateStatus($"Детектор завершён: точка найдена на индексе {index}");
                }
                else
                {
                    DetectionResultText.Text = "✗ Точка заплечника не найдена";
                    UpdateStatus("Детектор завершён: точка не найдена");
                }
            }
            catch (Exception ex)
            {
                DetectionResultText.Text = $"⚠ Ошибка: {ex.Message}";
                UpdateStatus($"Ошибка расчёта: {ex.Message}");
            }
        }
    }
}
