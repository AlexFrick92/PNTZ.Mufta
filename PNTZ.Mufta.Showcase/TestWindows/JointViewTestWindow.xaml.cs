using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.ViewModel.Joint;
using PNTZ.Mufta.Showcase.Helper;
using PNTZ.Mufta.Showcase.Data;
using Promatis.Core.Logging;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования контрола JointView
    /// </summary>
    public partial class JointViewTestWindow : Window
    {
        private JointViewModel _viewModel;
        private MockRecipeLoader _mockRecipeLoader;
        private RealRecipeLoader _realRecipeLoader;
        private MockJointProcessWorker _mockJointProcessWorker;
        private RealDataJointProcessWorker _realDataWorker;
        private TestResultsRepository _resultsRepository;
        private List<JointResultTable> _loadedResults;
        private IJointProcessTableWorker _currentWorker;
        private IRecipeTableLoader _currentRecipeLoader;

        public JointViewTestWindow()
        {
            InitializeComponent();
            InitializeViewModel();
            InitializeView();
        }

        private void InitializeView()
        {
            UpdateIntervalTextBox.Text = "10";
        }

        private void InitializeViewModel()
        {
            _mockRecipeLoader = new MockRecipeLoader();
            _realRecipeLoader = new RealRecipeLoader();
            _mockJointProcessWorker = new MockJointProcessWorker();
            _realDataWorker = new RealDataJointProcessWorker();

            _mockRecipeLoader.RecipeLoaded += OnMockRecipeLoaded;
            _mockRecipeLoader.RecipeLoaded += (s, r) => _mockJointProcessWorker.SetActualRecipe(r);

            _realRecipeLoader.RecipeLoaded += OnRealRecipeLoaded;
            _realRecipeLoader.RecipeLoaded += (s, r) => _realDataWorker.SetActualRecipe(r);

            // По умолчанию используем моковый воркер
            _currentWorker = _mockJointProcessWorker;
            _currentRecipeLoader = _mockRecipeLoader;
            _viewModel = new JointViewModel(_currentWorker, _currentRecipeLoader, new ConsoleLogger());
            JointView.DataContext = _viewModel;

            _mockJointProcessWorker.Initialize();
        }

        /// <summary>
        /// Обработчик успешной загрузки рецепта из мока (для обновления статуса)
        /// </summary>
        private void OnMockRecipeLoaded(object sender, JointRecipeTable recipe)
        {
            // Обновляем статус в UI (нужно вызвать в UI потоке)
            Dispatcher.Invoke(() =>
            {
                UpdateStatus($"Рецепт загружен: {recipe.Name} (Режим: {recipe.JointMode})");
            });
        }

        /// <summary>
        /// Обработчик успешной загрузки реального рецепта из БД (для обновления статуса)
        /// </summary>
        private void OnRealRecipeLoaded(object sender, JointRecipeTable recipe)
        {
            // Обновляем статус в UI (нужно вызвать в UI потоке)
            Dispatcher.Invoke(() =>
            {
                UpdateStatus($"Реальный рецепт загружен: {recipe.Name} (Режим: {recipe.JointMode})");
            });
        }

        public int UpdateInterval { get; set; }


        private void UpdateInterval_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                UpdateInterval = int.Parse(UpdateIntervalTextBox.Text);
            }
            catch
            {
                // Игнорируем ошибки парсинга
            }
        }

        /// <summary>
        /// Обновляет статусную строку
        /// </summary>
        private void UpdateStatus(string message)
        {
            StatusText.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        }

        private void BtnStartSimulation_Click(object sender, RoutedEventArgs e)
        {
            // Устанавливаем интервал обновления
            if (_currentWorker == _mockJointProcessWorker)
            {
                _mockJointProcessWorker.UpdateIntervalMs = UpdateInterval;
                _mockJointProcessWorker.Start();
                UpdateStatus($"Симуляция запущена с интервалом {UpdateInterval} мс (Mock)");
            }
            else if (_currentWorker == _realDataWorker)
            {
                _realDataWorker.UpdateIntervalMs = UpdateInterval;
                _realDataWorker.Start();
                UpdateStatus($"Воспроизведение реальных данных запущено с интервалом {UpdateInterval} мс");
            }
        }

        private void BtnStopSimulation_Click(object sender, RoutedEventArgs e)
        {
            // Останавливаем текущий воркер
            if (_currentWorker == _mockJointProcessWorker)
            {
                _mockJointProcessWorker.Stop();
            }
            else if (_currentWorker == _realDataWorker)
            {
                _realDataWorker.Stop();
            }

            UpdateStatus("Воспроизведение остановлено");
        }

        private void BtnLoadRecipeLength_Click(object sender, RoutedEventArgs e)
        {
            SwitchToMockWorker();
            UpdateStatus("Загрузка рецепта Length...");
            _mockRecipeLoader.LoadRecipeLength();
        }

        private void BtnLoadRecipeTorque_Click(object sender, RoutedEventArgs e)
        {
            SwitchToMockWorker();
            UpdateStatus("Загрузка рецепта Torque...");
            _mockRecipeLoader.LoadRecipeTorque();
        }

        private void BtnLoadRecipeTorqueLength_Click(object sender, RoutedEventArgs e)
        {
            SwitchToMockWorker();
            UpdateStatus("Загрузка рецепта Torque+Length...");
            _mockRecipeLoader.LoadRecipeTorqueLength();
        }

        private void BtnLoadRecipeTorqueShoulder_Click(object sender, RoutedEventArgs e)
        {
            SwitchToMockWorker();
            UpdateStatus("Загрузка рецепта Torque+Shoulder...");
            _mockRecipeLoader.LoadRecipeTorqueShoulder();
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
                    JointResultTable realResult = selectedTable;

                    // Загружаем данные в RealDataWorker
                    _realDataWorker.LoadRealData(realResult);

                    // Переключаемся на RealDataWorker
                    SwitchToRealDataWorker();

                    // Загружаем рецепт из результата через RealRecipeLoader
                    _realRecipeLoader.LoadRecipe(realResult.Recipe);

                    UpdateStatus($"Загружены реальные данные: {realResult.Recipe.Name}, {realResult.PointSeries.Count} точек");
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Ошибка загрузки данных: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Переключение на использование реального воркера с данными из БД
        /// </summary>
        private void SwitchToRealDataWorker()
        {
            if (_currentWorker == _realDataWorker && _currentRecipeLoader == _realRecipeLoader)
                return; // Уже используем реальный воркер и загрузчик

            // Останавливаем текущий воркер, если он работает
            if (_currentWorker == _mockJointProcessWorker)
            {
                _mockJointProcessWorker.Stop();
            }
            else if (_currentWorker == _realDataWorker)
            {
                _realDataWorker.Stop();
            }

            // Переключаем воркер и загрузчик
            _currentWorker = _realDataWorker;
            _currentRecipeLoader = _realRecipeLoader;

            // Пересоздаем ViewModel с новым воркером и загрузчиком
            _viewModel = new JointViewModel(_currentWorker, _currentRecipeLoader, new ConsoleLogger());
            JointView.DataContext = _viewModel;
        }

        /// <summary>
        /// Переключение обратно на моковый воркер
        /// </summary>
        private void SwitchToMockWorker()
        {
            if (_currentWorker == _mockJointProcessWorker && _currentRecipeLoader == _mockRecipeLoader)
                return; // Уже используем моковый воркер и загрузчик

            // Останавливаем текущий воркер
            if (_currentWorker == _realDataWorker)
            {
                _realDataWorker.Stop();
            }
            else if (_currentWorker == _mockJointProcessWorker)
            {
                _mockJointProcessWorker.Stop();
            }

            // Переключаем воркер и загрузчик
            _currentWorker = _mockJointProcessWorker;
            _currentRecipeLoader = _mockRecipeLoader;

            // Пересоздаем ViewModel с новым воркером и загрузчиком
            _viewModel = new JointViewModel(_currentWorker, _currentRecipeLoader, new ConsoleLogger());
            JointView.DataContext = _viewModel;

            _mockJointProcessWorker.Initialize();
        }
    }
}
