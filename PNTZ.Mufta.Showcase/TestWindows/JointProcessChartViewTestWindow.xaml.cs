using System;
using System.Windows;
using System.Windows.Threading;
using PNTZ.Mufta.Showcase.Helper;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.ViewModel.Joint;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования JointProcessChartView
    /// </summary>
    public partial class JointProcessChartViewTestWindow : Window
    {
        private JointProcessChartViewModel _viewModel;
        private JointRecipe _currentRecipe;
        private JointResult _currentJointResult;

        // Симуляция данных
        private DispatcherTimer _simulationTimer;
        private Random _random = new Random();
        private int _currentTimeStamp = 0;
        private float _currentTorque = 0;
        private float _currentLength = 0;
        private float _currentTurns = 0;
        private TqTnLenPoint _lastPoint = null;

        // Константы симуляции
        private const int SIMULATION_INTERVAL_MS = 10;
        private const int SIMULATION_DURATION_MS = 15000;
        private const float MAX_TURNS = 2.5f;

        // Коэффициенты превышения пределов для тестирования auto-expand
        private const float TORQUE_OVERSHOOT = 1.25f;  // Превышение момента на 25%
        private const float LENGTH_OVERSHOOT = 1.15f;  // Превышение длины на 15%
        private const float TURNS_OVERSHOOT = 1.3f;    // Превышение оборотов на 30%
        private const float RPM_MAX = 40f;             // Максимальные обороты в минуту

        public JointProcessChartViewTestWindow()
        {
            InitializeComponent();
            InitializeViewModel();
            InitializeSimulationTimer();
        }

        /// <summary>
        /// Инициализация ViewModel для JointProcessChartView
        /// </summary>
        private void InitializeViewModel()
        {
            _viewModel = new JointProcessChartViewModel();
            JointChartView.DataContext = _viewModel;
        }

        /// <summary>
        /// Инициализация таймера для симуляции
        /// </summary>
        private void InitializeSimulationTimer()
        {
            _simulationTimer = new DispatcherTimer();
            _simulationTimer.Interval = TimeSpan.FromMilliseconds(SIMULATION_INTERVAL_MS);
            _simulationTimer.Tick += SimulationTimer_Tick;
        }

        /// <summary>
        /// Загрузить тестовый рецепт по длине
        /// </summary>
        private void LoadRecipeLength_Click(object sender, RoutedEventArgs e)
        {
            var recipe = RecipeHelper.CreateTestRecipeLength();
            _currentRecipe = recipe;
            _viewModel.UpdateRecipe(recipe);
            StatusText.Text = $"Загружен рецепт: {recipe.Name} (режим: {recipe.JointMode})";
        }

        /// <summary>
        /// Загрузить тестовый рецепт по моменту
        /// </summary>
        private void LoadRecipeTorque_Click(object sender, RoutedEventArgs e)
        {
            var recipe = RecipeHelper.CreateTestRecipeTorque();
            _currentRecipe = recipe;
            _viewModel.UpdateRecipe(recipe);
            StatusText.Text = $"Загружен рецепт: {recipe.Name} (режим: {recipe.JointMode})";
        }

        /// <summary>
        /// Загрузить тестовый рецепт по моменту и длине
        /// </summary>
        private void LoadRecipeTorqueLength_Click(object sender, RoutedEventArgs e)
        {
            var recipe = RecipeHelper.CreateTestRecipeTorqueLength();
            _currentRecipe = recipe;
            _viewModel.UpdateRecipe(recipe);
            StatusText.Text = $"Загружен рецепт: {recipe.Name} (режим: {recipe.JointMode})";
        }

        /// <summary>
        /// Загрузить тестовый рецепт по моменту до упора
        /// </summary>
        private void LoadRecipeTorqueShoulder_Click(object sender, RoutedEventArgs e)
        {
            var recipe = RecipeHelper.CreateTestRecipeTorqueShoulder();
            _currentRecipe = recipe;
            _viewModel.UpdateRecipe(recipe);
            StatusText.Text = $"Загружен рецепт: {recipe.Name} (режим: {recipe.JointMode})";
        }

        /// <summary>
        /// Обработчик появления трубы на позиции
        /// </summary>
        private void PipeAppear_Click(object sender, RoutedEventArgs e)
        {
            // Проверка загрузки рецепта
            if (_currentRecipe == null)
            {
                StatusText.Text = "Ошибка: Сначала загрузите рецепт!";
                return;
            }

            // Парсинг значения MVS_Len из TextBox
            if (!float.TryParse(TxtMvsLen.Text, out float mvsLenMm))
            {
                StatusText.Text = "Ошибка: Некорректное значение MVS_Len!";
                return;
            }

            // Создание нового JointResult
            _currentJointResult = new JointResult(_currentRecipe)
            {
                MVS_Len = mvsLenMm / 1000f, // Преобразуем мм в метры
                StartTimeStamp = DateTime.Now
            };

            // Вызываем метод UpdatePipeAppear у ViewModel
            _viewModel.UpdatePipeAppear(_currentJointResult);

            StatusText.Text = $"Труба появилась на позиции: {mvsLenMm:F1} мм (MVS_Len = {_currentJointResult.MVS_Len:F4} м)";
        }

        #region Обработчики симуляции

        /// <summary>
        /// Начать симуляцию записи данных
        /// </summary>
        private void StartSimulation_Click(object sender, RoutedEventArgs e)
        {
            if (_currentRecipe == null)
            {
                StatusText.Text = "Ошибка: Сначала загрузите рецепт!";
                return;
            }

            // Инициализация параметров симуляции
            _currentTimeStamp = 0;
            _currentTorque = 0;
            _currentLength = _currentJointResult?.MVS_Len_mm ?? 0; // Начинаем от позиции трубы
            _currentTurns = 0;
            _lastPoint = null;

            // Запуск таймера
            _simulationTimer.Start();
            StatusText.Text = "Симуляция запущена (15 секунд)";
        }

        /// <summary>
        /// Остановить симуляцию
        /// </summary>
        private void StopSimulation_Click(object sender, RoutedEventArgs e)
        {
            _simulationTimer.Stop();

            // Подгоняем границы графиков под финальные данные
            if (_currentJointResult != null)
            {
                _currentJointResult.ResultTotal = 2;
                _currentJointResult.FinishTimeStamp = DateTime.Now;
                _viewModel.UpdateJointFinished(_currentJointResult);

            }

            StatusText.Text = $"Симуляция остановлена (точек: {_viewModel.TqTnLenPoints.Count})";
        }

        /// <summary>
        /// Сброс графиков и данных симуляции
        /// </summary>
        private void ResetSimulation_Click(object sender, RoutedEventArgs e)
        {
            _simulationTimer.Stop();
            _viewModel.TqTnLenPoints.Clear();
            _currentTimeStamp = 0;
            _currentTorque = 0;
            _currentLength = _currentJointResult?.MVS_Len_mm ?? 0;
            _currentTurns = 0;
            _lastPoint = null;
            StatusText.Text = "Графики сброшены";
        }

        /// <summary>
        /// Обработчик тика таймера для генерации точек
        /// </summary>
        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            // Проверка окончания симуляции
            if (_currentTimeStamp >= SIMULATION_DURATION_MS)
            {
                _simulationTimer.Stop();

                // Подгоняем границы графиков под финальные данные
                _currentJointResult.FinishTimeStamp = DateTime.Now;
                _currentJointResult.ResultTotal = 1;
                _viewModel.UpdateJointFinished(_currentJointResult);

                StatusText.Text = $"Симуляция завершена ({_viewModel.TqTnLenPoints.Count} точек)";
                return;
            }

            // Прогресс симуляции (0.0 - 1.0)
            float progress = (float)_currentTimeStamp / SIMULATION_DURATION_MS;

            // Генерация данных с линейным ростом и шумом
            _currentTorque = GenerateTorqueValue(progress);
            _currentLength = GenerateLengthValue(progress) / 1000;
            _currentTurns = GenerateTurnsValue(progress);

            // Создание новой точки
            var newPoint = new TqTnLenPoint
            {
                TimeStamp = _currentTimeStamp,
                Torque = _currentTorque,
                Length = _currentLength,
                Turns = _currentTurns,
                TurnsPerMinute = 0
            };

            // Расчет TurnsPerMinute с вариацией
            if (_lastPoint != null)
            {
                newPoint.TurnsPerMinute = GenerateRPMValue(progress);
            }

            // Добавление точки в коллекцию
            _viewModel.TqTnLenPoints.Add(newPoint);
            _lastPoint = newPoint;

            // Инкремент времени
            _currentTimeStamp += SIMULATION_INTERVAL_MS;

            // Обновление статуса
            StatusText.Text = $"Симуляция: {_currentTimeStamp}/{SIMULATION_DURATION_MS} мс ({_viewModel.TqTnLenPoints.Count} точек)";
        }

        /// <summary>
        /// Генерация значения момента с линейным ростом и шумом.
        /// Превышает MU_Tq_Max на 25% для тестирования auto-expand границ.
        /// </summary>
        private float GenerateTorqueValue(float progress)
        {
            if (_currentRecipe == null) return 0;

            // Используем превышенный максимум для тестирования расширения границ
            float targetMaxTorque = _currentRecipe.MU_Tq_Max * TORQUE_OVERSHOOT;
            float linearValue = targetMaxTorque * progress;
            float noise = (float)(_random.NextDouble() * 2 - 1) * (targetMaxTorque * 0.05f); // ±5% шум

            // Иногда уходим в отрицательные значения в начале для тестирования Min
            if (progress < 0.1f)
            {
                linearValue = (float)(_random.NextDouble() * 2 - 1) * (_currentRecipe.MU_Tq_Min * 0.3f);
            }

            return linearValue + noise;
        }

        /// <summary>
        /// Генерация значения длины с линейным ростом (без шума).
        /// Превышает MU_Len_Max на 15% для тестирования auto-expand границ.
        /// </summary>
        private float GenerateLengthValue(float progress)
        {
            if (_currentRecipe == null) return 0;

            // Начальная позиция (MVS_Len_mm) + дельта до превышенного максимума
            float startLength = _currentJointResult?.MVS_Len_mm ?? 0;
            float targetMaxLength = _currentRecipe.MU_Len_Max * LENGTH_OVERSHOOT;
            float deltaLength = targetMaxLength - startLength;

            return startLength + (deltaLength * progress);
        }

        /// <summary>
        /// Генерация значения оборотов с линейным ростом (без шума).
        /// Превышает MAX_TURNS на 30% для тестирования auto-expand границ.
        /// </summary>
        private float GenerateTurnsValue(float progress)
        {
            return MAX_TURNS * TURNS_OVERSHOOT * progress;
        }

        /// <summary>
        /// Генерация значения оборотов в минуту с вариацией.
        /// Достигает до 40 RPM для тестирования auto-expand границ.
        /// </summary>
        private float GenerateRPMValue(float progress)
        {
            // Плавное изменение RPM с небольшим шумом
            float baseRPM = RPM_MAX * (0.7f + 0.3f * progress);
            float noise = (float)(_random.NextDouble() * 2 - 1) * (RPM_MAX * 0.1f); // ±10% шум
            return Math.Max(0, baseRPM + noise);
        }

        #endregion        
    }
}
