using System;
using System.Windows;
using System.Windows.Threading;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.ViewModel.Joint;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования контрола JointProcessDataView
    /// </summary>
    public partial class JointProcessDataViewTestWindow : Window
    {
        private JointProcessDataViewModel _viewModel;

        public JointProcessDataViewTestWindow()
        {
            InitializeComponent();
            InitializeViewModel();
            InitializeView();
            InitializeSimulation();
        }
        private void InitializeView()
        {
            UpdateIntervalTextBox.Text = "100";// UpdateInterval.ToString();
        }
        private void InitializeViewModel()
        {
            _viewModel = new JointProcessDataViewModel();
            JointProcessDataView.DataContext = _viewModel;
        }

        public int UpdateInterval { get; set; } // Интервал симуляции по умолчанию 100 мс        

        //Состояние симуляции
        private bool _isSimulationRunning;
        public bool IsSimulationRunning
        {
            get { return _isSimulationRunning; }
            set { _isSimulationRunning = value; }
        }

        //Таймер для симуляции
        private DispatcherTimer _simulationTimer;

        //Счетчики и параметры симуляции
        private double _sinePhase = 0;  // Фаза для синусоиды (Torque)
        private float _currentLength = 0;  // Текущая длина в метрах
        private float _currentTurns = 0;  // Текущие обороты
        private bool _lengthIncreasing = true;  // Направление изменения Length
        private bool _turnsIncreasing = true;  // Направление изменения Turns
        private int _currentTimeStamp = 0;  // Временная метка

        //Константы симуляции
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
            _viewModel.ActualPoint = new TqTnLenPoint
            {
                Torque = 0,
                Length = 0,
                Turns = 0,
                TurnsPerMinute = 0,
                TimeStamp = 0
            };
        }

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
            _viewModel.BeginNewJointing();
        }

        public void StopSimulation()
        {
            if (!IsSimulationRunning)
                return;

            _simulationTimer.Stop();
            IsSimulationRunning = false;

            var result = new JointResult(new JointRecipe());
            if(_viewModel.ActualPoint.Torque < 5000)
            {
                result.ResultTotal = 2; // Плохой результат
            }
            else
            {
                result.ResultTotal = 1; // Хороший результат
            }

            _viewModel.FinishJointing(result);
        }

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            GenerateNextPoint();
            UpdateStatus($"Simulation running. TimeStamp: {_viewModel.ActualPoint.TimeStamp} ms");
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

            // Обновление ActualPoint
            _viewModel.ActualPoint = new TqTnLenPoint
            {
                Torque = torque,
                Length = _currentLength,
                Turns = _currentTurns,
                TurnsPerMinute = 60.0f,  // Фиксированное значение для примера
                TimeStamp = _currentTimeStamp
            };
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

        private void BtnLoadRecipe_Click(object sender, RoutedEventArgs e)
        {
            var recipe = new JointRecipe();
            _viewModel.UpdateRecipe(recipe);
            UpdateStatus("Рецепт загружен с дефолтными параметрами.");
        }
    }
}
