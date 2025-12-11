using System;
using System.Windows;
using System.Windows.Threading;
using PNTZ.Mufta.TPCApp.Domain;
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
        private MockJointProcessWorker _mockJointProcessWorker;

        public JointViewTestWindow()
        {
            InitializeComponent();
            InitializeViewModel();
            InitializeView();
            InitializeSimulation();
        }

        private void InitializeView()
        {
            UpdateIntervalTextBox.Text = "10";
        }

        private void InitializeViewModel()
        {
            _mockRecipeLoader = new MockRecipeLoader();
            _mockJointProcessWorker = new MockJointProcessWorker();
            _mockRecipeLoader.RecipeLoaded += OnMockRecipeLoaded;

            _viewModel = new JointViewModel(_mockJointProcessWorker, _mockRecipeLoader, new ConsoleLogger());
            JointView.DataContext = _viewModel;
        }

        /// <summary>
        /// Обработчик успешной загрузки рецепта из мока (для обновления статуса и Recipe)
        /// </summary>
        private void OnMockRecipeLoaded(object sender, JointRecipe recipe)
        {
            // Обновляем Recipe для использования в StopSimulation
            Recipe = recipe;
            // Обновляем статус в UI (нужно вызвать в UI потоке)
            Dispatcher.Invoke(() =>
            {
                UpdateStatus($"Рецепт загружен: {recipe.Name} (Режим: {recipe.JointMode})");
            });
        }

        public int UpdateInterval { get; set; }

        // Состояние симуляции
        private bool _isSimulationRunning;
        public bool IsSimulationRunning
        {
            get { return _isSimulationRunning; }
            set { _isSimulationRunning = value; }
        }

        // Таймер для симуляции
        private DispatcherTimer _simulationTimer;

        // Счетчики и параметры симуляции
        private double _sinePhase = 0;  // Фаза для синусоиды (Torque)
        private float _currentLength = 0;  // Текущая длина в метрах
        private float _currentTurns = 0;  // Текущие обороты
        private bool _lengthIncreasing = true;  // Направление изменения Length
        private bool _turnsIncreasing = true;  // Направление изменения Turns
        private int _currentTimeStamp = 0;  // Временная метка

        // Константы симуляции
        private const float MAX_LENGTH_M = 0.150f;  // 150 мм = 0.15 м
        private const float LENGTH_STEP_M = 0.0005f;  // 0.5 мм = 0.0005 м
        private const float MAX_TURNS = 5.0f;
        private const float TURNS_STEP = 0.05f;
        private const float MAX_TORQUE = 20000f;

        private void InitializeSimulation()
        {
            _simulationTimer = new DispatcherTimer();
            _simulationTimer.Tick += SimulationTimer_Tick;

            // Инициализируем ActualPoint нулевыми значениями
            _viewModel.JointProcessDataViewModel.ActualPoint = new TqTnLenPoint
            {
                Torque = 0,
                Length = 0,
                Turns = 0,
                TurnsPerMinute = 0,
                TimeStamp = 0
            };
        }

        public JointRecipe Recipe { get; set; }

        public void StartSimulation()
        {
            if (IsSimulationRunning)
                return;

            _simulationTimer.Interval = TimeSpan.FromMilliseconds(UpdateInterval);
            _simulationTimer.Start();
            IsSimulationRunning = true;

            // Сброс счетчиков
            _sinePhase = 0;
            _currentLength = 0;
            _currentTurns = 0;
            _lengthIncreasing = true;
            _turnsIncreasing = true;
            _currentTimeStamp = 0;

            // Начинаем процесс навёртки на обоих дочерних ViewModel
            _viewModel.JointProcessDataViewModel.BeginNewJointing();
            _viewModel.JointProcessChartViewModel.TqTnLenPoints.Clear();
        }

        public void StopSimulation()
        {
            if (!IsSimulationRunning)
                return;

            _simulationTimer.Stop();
            IsSimulationRunning = false;

            var result = new JointResult(Recipe);
            if (_viewModel.JointProcessDataViewModel.ActualPoint.Torque < 5000)
            {
                result.ResultTotal = 2; // Плохой результат
                result.EvaluationVerdict = new EvaluationVerdict() { LentghOk = true, ShoulderOk = true, TorqueOk = false };
            }
            else
            {
                result.ResultTotal = 1; // Хороший результат
                result.EvaluationVerdict = new EvaluationVerdict() { LentghOk = true, ShoulderOk = true, TorqueOk = true };
            }

            // Завершаем процесс на обоих дочерних ViewModel
            _viewModel.JointProcessDataViewModel.FinishJointing(result);
            _viewModel.JointProcessChartViewModel.UpdateJointFinished(result);
        }

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            GenerateNextPoint();
            UpdateStatus($"Simulation running. TimeStamp: {_viewModel.JointProcessDataViewModel.ActualPoint.TimeStamp} ms");
        }

        private void GenerateNextPoint()
        {
            // Генерация Torque - синусоида от 0 до 20000
            _sinePhase += 0.1;
            float torque = (float)((Math.Sin(_sinePhase) + 1) / 2 * MAX_TORQUE);

            // Генерация Length - линейный рост/уменьшение
            if (_lengthIncreasing)
            {
                _currentLength += LENGTH_STEP_M;
                if (_currentLength >= MAX_LENGTH_M)
                {
                    _currentLength = MAX_LENGTH_M;
                    _lengthIncreasing = false;
                }
            }
            else
            {
                _currentLength -= LENGTH_STEP_M;
                if (_currentLength <= 0)
                {
                    _currentLength = 0;
                    _lengthIncreasing = true;
                }
            }

            // Генерация Turns - линейный рост/уменьшение
            if (_turnsIncreasing)
            {
                _currentTurns += TURNS_STEP;
                if (_currentTurns >= MAX_TURNS)
                {
                    _currentTurns = MAX_TURNS;
                    _turnsIncreasing = false;
                }
            }
            else
            {
                _currentTurns -= TURNS_STEP;
                if (_currentTurns <= 0)
                {
                    _currentTurns = 0;
                    _turnsIncreasing = true;
                }
            }

            // Увеличение временной метки
            _currentTimeStamp += UpdateInterval;

            // Создаём новую точку
            var newPoint = new TqTnLenPoint
            {
                Torque = torque,
                Length = _currentLength,
                Turns = _currentTurns,
                TurnsPerMinute = 60.0f,  // Фиксированное значение для примера
                TimeStamp = _currentTimeStamp
            };

            // Обновление ActualPoint на обоих дочерних ViewModel
            _viewModel.JointProcessDataViewModel.ActualPoint = newPoint;
            _viewModel.JointProcessChartViewModel.TqTnLenPoints.Add(newPoint);
        }

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
            StartSimulation();
        }

        private void BtnStopSimulation_Click(object sender, RoutedEventArgs e)
        {
            StopSimulation();
        }

        private void BtnLoadRecipeLength_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Загрузка рецепта Length...");
            _mockRecipeLoader.LoadRecipeLength();
        }

        private void BtnLoadRecipeTorque_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Загрузка рецепта Torque...");
            _mockRecipeLoader.LoadRecipeTorque();
        }

        private void BtnLoadRecipeTorqueLength_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Загрузка рецепта Torque+Length...");
            _mockRecipeLoader.LoadRecipeTorqueLength();
        }

        private void BtnLoadRecipeTorqueShoulder_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Загрузка рецепта Torque+Shoulder...");
            _mockRecipeLoader.LoadRecipeTorqueShoulder();
        }
    }
}
