using System;
using System.Windows;
using System.Windows.Threading;
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
            var recipe = CreateTestRecipeLength();
            _currentRecipe = recipe;
            _viewModel.UpdateRecipe(recipe);
            StatusText.Text = $"Загружен рецепт: {recipe.Name} (режим: {recipe.JointMode})";
        }

        /// <summary>
        /// Загрузить тестовый рецепт по моменту
        /// </summary>
        private void LoadRecipeTorque_Click(object sender, RoutedEventArgs e)
        {
            var recipe = CreateTestRecipeTorque();
            _currentRecipe = recipe;
            _viewModel.UpdateRecipe(recipe);
            StatusText.Text = $"Загружен рецепт: {recipe.Name} (режим: {recipe.JointMode})";
        }

        /// <summary>
        /// Загрузить тестовый рецепт по моменту и длине
        /// </summary>
        private void LoadRecipeTorqueLength_Click(object sender, RoutedEventArgs e)
        {
            var recipe = CreateTestRecipeTorqueLength();
            _currentRecipe = recipe;
            _viewModel.UpdateRecipe(recipe);
            StatusText.Text = $"Загружен рецепт: {recipe.Name} (режим: {recipe.JointMode})";
        }

        /// <summary>
        /// Загрузить тестовый рецепт по моменту до упора
        /// </summary>
        private void LoadRecipeTorqueShoulder_Click(object sender, RoutedEventArgs e)
        {
            var recipe = CreateTestRecipeTorqueShoulder();
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

        /// <summary>
        /// Создать базовый рецепт с заполненными всеми полями
        /// </summary>
        private JointRecipe CreateBaseRecipe(Action<JointRecipe> configure)
        {
            var recipe = new JointRecipe
            {
                Id = Guid.NewGuid(),
                Name = "TEST_BASE",
                JointMode = JointMode.Length,
                SelectedThreadType = ThreadType.RIGHT,

                // Общие данные
                HEAD_OPEN_PULSES = 100f,
                TURNS_BREAK = 0.5f,
                PLC_PROG_NR = 1,
                LOG_NO = 1,
                Tq_UNIT = 1,
                Thread_step = 25.4f,
                PIPE_TYPE = "TEST_PIPE",

                // Параметры муфты
                Box_Moni_Time = 5000,
                Box_Len_Min = 10f,
                Box_Len_Max = 50f,

                // Параметры преднавёртки
                Pre_Moni_Time = 10000,
                Pre_Len_Max = 100f,
                Pre_Len_Min = 20f,

                // Параметры силового свинчивания общие
                MU_Moni_Time = 15000,
                MU_Tq_Ref = 5000f,
                MU_Tq_Save = 4500f,
                MU_TqSpeedRed_1 = 3000f,
                MU_TqSpeedRed_2 = 4000f,
                MU_Tq_Dump = 2000f,
                MU_Tq_Max = 8000f,
                MU_Tq_Min = 3000f,
                MU_Tq_Opt = 6000f,
                MU_TqShoulder_Min = 2500f,
                MU_TqShoulder_Max = 3500f,

                // Параметры силового свинчивания по длине
                MU_Len_Speed_1 = 50f,
                MU_Len_Speed_2 = 30f,
                MU_Len_Dump = 150f,
                MU_Len_Min = 108f,
                MU_Len_Max = 116f,

                // Параметры силового свинчивания по J
                MU_JVal_Speed_1 = 100f,
                MU_JVal_Speed_2 = 80f,
                MU_JVal_Dump = 500f,
                MU_JVal_Min = 1000f,
                MU_JVal_Max = 2000f,

                TimeStamp = DateTime.Now
            };

            configure?.Invoke(recipe);
            return recipe;
        }

        /// <summary>
        /// Создать тестовый рецепт по длине
        /// </summary>
        private JointRecipe CreateTestRecipeLength()
        {
            return CreateBaseRecipe(recipe =>
            {
                recipe.Name = "TEST_LENGTH";
                recipe.JointMode = JointMode.Length;
                recipe.PLC_PROG_NR = 1;
                recipe.LOG_NO = 1;

                // Средние параметры между Length и Torque              
                recipe.MU_Moni_Time = 16000;

                recipe.MU_Tq_Ref = 6000f;
                recipe.MU_Tq_Save = 5500f;
                recipe.MU_Tq_Dump = 2500f;
                recipe.MU_Tq_Max = 9000f;
                recipe.MU_Tq_Min = 3500f;
                recipe.MU_Tq_Opt = 7000f;

                recipe.MU_Len_Speed_1 = 55f;
                recipe.MU_Len_Speed_2 = 35f;
                recipe.MU_Len_Min = 108f;
                recipe.MU_Len_Max = 112f;
                recipe.MU_Len_Dump = 110f;
            });
        }

        /// <summary>
        /// Создать тестовый рецепт по моменту
        /// </summary>
        private JointRecipe CreateTestRecipeTorque()
        {
            return CreateBaseRecipe(recipe =>
            {
                recipe.Name = "TEST_TORQUE";
                recipe.JointMode = JointMode.Torque;
                recipe.PLC_PROG_NR = 2;
                recipe.LOG_NO = 2;
                recipe.PIPE_TYPE = "TEST_PIPE_TQ";
               
                recipe.MU_Moni_Time = 18000;
                recipe.MU_Tq_Ref = 7000f;
                recipe.MU_Tq_Save = 6500f;
                recipe.MU_TqSpeedRed_1 = 4000f;
                recipe.MU_TqSpeedRed_2 = 5500f;
                recipe.MU_Tq_Dump = 3000f;
                recipe.MU_Tq_Max = 10000f;
                recipe.MU_Tq_Min = 4000f;
                recipe.MU_Tq_Opt = 8000f;
            });
        }

        /// <summary>
        /// Создать тестовый рецепт по моменту и длине
        /// </summary>
        private JointRecipe CreateTestRecipeTorqueLength()
        {
            return CreateBaseRecipe(recipe =>
            {
                recipe.Name = "TEST_TORQUE_LENGTH";
                recipe.JointMode = JointMode.TorqueLength;
                recipe.PLC_PROG_NR = 3;
                recipe.LOG_NO = 3;
                recipe.PIPE_TYPE = "TEST_PIPE_TQLEN";

                // Средние параметры между Length и Torque              
                recipe.MU_Moni_Time = 16000;
                recipe.MU_Tq_Ref = 6000f;
                recipe.MU_Tq_Save = 5500f;
                recipe.MU_Tq_Dump = 2500f;
                recipe.MU_Tq_Max = 9000f;
                recipe.MU_Tq_Min = 3500f;
                recipe.MU_Tq_Opt = 7000f;

                recipe.MU_Len_Speed_1 = 55f;
                recipe.MU_Len_Speed_2 = 35f;
                recipe.MU_Len_Min = 108f;
                recipe.MU_Len_Max = 112f;
                recipe.MU_Len_Dump = 110f;
            });
        }

        /// <summary>
        /// Создать тестовый рецепт по моменту до упора
        /// </summary>
        private JointRecipe CreateTestRecipeTorqueShoulder()
        {
            return CreateBaseRecipe(recipe =>
            {
                recipe.Name = "TEST_TORQUE_SHOULDER";
                recipe.JointMode = JointMode.TorqueShoulder;
                recipe.PLC_PROG_NR = 3;
                recipe.LOG_NO = 3;
                recipe.PIPE_TYPE = "TEST_PIPE_TQSHL";

                // Средние параметры между Length и Torque              
                recipe.MU_Moni_Time = 16000;
                recipe.MU_Tq_Ref = 6000f;
                recipe.MU_Tq_Save = 5500f;
                recipe.MU_TqSpeedRed_1 = 3500f;
                recipe.MU_TqSpeedRed_2 = 4500f;
                recipe.MU_Tq_Dump = 6700;
                recipe.MU_Tq_Max = 9000f;
                recipe.MU_Tq_Min = 2500f;
                recipe.MU_Tq_Opt = 7000f;
                recipe.MU_TqShoulder_Min = 4000f;
                recipe.MU_TqShoulder_Max = 6200f;              
            });         
        }
    }
}
