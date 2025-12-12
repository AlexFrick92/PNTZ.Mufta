using System;
using System.Threading;
using System.Threading.Tasks;
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
        private Random _random = new Random();
        private int _currentTimeStamp = 0;
        private float _currentTorque = 0;
        private float _currentLength = 0;
        private float _currentTurns = 0;
        private TqTnLenPoint _lastPoint = null;

        // Многопоточная симуляция
        private TqTnLenPoint _latestPoint = null;
        private object _pointLock = new object();
        private CancellationTokenSource _simulationCts;
        private Task _simulationTask;

        // Константы симуляции
        private const int SIMULATION_INTERVAL_MS = 10;
        private const int SIMULATION_DURATION_MS = 15000;
        private const float MAX_TURNS = 2.5f;

        // Коэффициенты превышения пределов для тестирования auto-expand
        private const float TORQUE_OVERSHOOT = 1.25f;  // Превышение момента на 25%
        private const float LENGTH_OVERSHOOT = 1.15f;  // Превышение длины на 15%
        private const float TURNS_OVERSHOOT = 1.3f;    // Превышение оборотов на 30%
        private const float RPM_MAX = 40f;             // Максимальные обороты в минуту

        private int _pointCount = 0;

        public JointProcessChartViewTestWindow()
        {
            InitializeComponent();
            InitializeViewModel();
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

            // Вызываем метод PipeAppear у ViewModel
            _viewModel.PipeAppear(_currentJointResult);

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

            // Создание CancellationTokenSource для управления фоновым потоком
            _simulationCts = new CancellationTokenSource();

            // Очистка предыдущей точки
            lock (_pointLock)
            {
                _latestPoint = null;
            }
            _viewModel.RecordingBegin();
            // Запуск фоновой генерации точек
            _simulationTask = Task.Run(() => GenerateSimulationDataInBackground(_simulationCts.Token));

            // Запуск таймера для обновления UI
            //_simulationTimer.Start();
            StatusText.Text = "Симуляция запущена (15 секунд)";
        }

        /// <summary>
        /// Остановить симуляцию
        /// </summary>
        private void StopSimulation_Click(object sender, RoutedEventArgs e)
        {
            // Остановка фонового потока через CancellationToken
            if (_simulationCts != null)
            {
                _simulationCts.Cancel();
            }

            // Ожидание завершения фоновой задачи
            if (_simulationTask != null)
            {
                try
                {
                    _simulationTask.Wait(1000); // Ждем максимум 1 секунду
                }
                catch (AggregateException)
                {
                    // Игнорируем исключения отмены
                }
            }

            // Подгоняем границы графиков под финальные данные
            if (_currentJointResult != null)
            {
                _currentJointResult.ResultTotal = 2;
                _currentJointResult.FinishTimeStamp = DateTime.Now;
                _viewModel.FinishJointing(_currentJointResult);
            }

            // Очистка ресурсов
            _simulationCts?.Dispose();
            _simulationCts = null;
            _simulationTask = null;

            StatusText.Text = $"Симуляция остановлена (точек: {_pointCount})";
        }

        /// <summary>
        /// Сброс графиков и данных симуляции
        /// </summary>
        private void ResetSimulation_Click(object sender, RoutedEventArgs e)
        {
            // Остановка фонового потока если он работает
            if (_simulationCts != null)
            {
                _simulationCts.Cancel();
            }

            // Ожидание завершения фоновой задачи
            if (_simulationTask != null)
            {
                try
                {
                    _simulationTask.Wait(1000);
                }
                catch (AggregateException)
                {
                    // Игнорируем исключения отмены
                }
            }

            // Очистка ресурсов многопоточной симуляции
            _simulationCts?.Dispose();
            _simulationCts = null;
            _simulationTask = null;

            lock (_pointLock)
            {
                _latestPoint = null;
            }

            // Очистка данных
            _viewModel.ClearCharts();
            _pointCount = 0;
            _currentTimeStamp = 0;
            _currentTorque = 0;
            _currentLength = _currentJointResult?.MVS_Len_mm ?? 0;
            _currentTurns = 0;
            _lastPoint = null;

            StatusText.Text = "Графики сброшены";
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

        /// <summary>
        /// Фоновая генерация точек симуляции в отдельном потоке
        /// </summary>
        private async Task GenerateSimulationDataInBackground(CancellationToken cancellationToken)
        {
            int currentTimeStamp = 0;
            float currentTorque = 0;
            float currentLength = _currentJointResult?.MVS_Len_mm ?? 0;
            float currentTurns = 0;
            TqTnLenPoint lastPoint = null;

            try
            {
                while (currentTimeStamp < SIMULATION_DURATION_MS && !cancellationToken.IsCancellationRequested)
                {
                    // Прогресс симуляции (0.0 - 1.0)
                    float progress = (float)currentTimeStamp / SIMULATION_DURATION_MS;

                    // Генерация данных с линейным ростом и шумом
                    currentTorque = GenerateTorqueValue(progress);
                    currentLength = GenerateLengthValue(progress) / 1000;
                    currentTurns = GenerateTurnsValue(progress);

                    // Создание новой точки
                    var newPoint = new TqTnLenPoint
                    {
                        TimeStamp = currentTimeStamp,
                        Torque = currentTorque,
                        Length = currentLength,
                        Turns = currentTurns,
                        TurnsPerMinute = 0
                    };

                    // Расчет TurnsPerMinute с вариацией
                    if (lastPoint != null)
                    {
                        newPoint.TurnsPerMinute = GenerateRPMValue(progress);
                    }

                    // Сохраняем точку в поле (потокобезопасно)
                    lock (_pointLock)
                    {
                        _latestPoint = newPoint;
                    }

                    lastPoint = newPoint;
                    currentTimeStamp += SIMULATION_INTERVAL_MS;
                    _viewModel.TqTnLenPointsQueue.Enqueue(lastPoint);
                    _pointCount++;

                    // Ждем интервал симуляции
                    await Task.Delay(SIMULATION_INTERVAL_MS, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Нормальная остановка через CancellationToken
            }
        }

        #endregion        
    }
}
