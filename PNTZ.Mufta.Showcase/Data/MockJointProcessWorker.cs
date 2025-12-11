using System;
using System.Threading;
using System.Threading.Tasks;
using PNTZ.Mufta.TPCApp.Domain;

namespace PNTZ.Mufta.Showcase.Data
{
    /// <summary>
    /// Тестовая реализация IJointProcessWorker для симуляции процесса свинчивания
    /// </summary>
    public class MockJointProcessWorker : IJointProcessWorker
    {
        private readonly Random _random = new Random();
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning;

        // Параметры симуляции
        private const float PRE_MAKEUP_LENGTH_MAX = 0.750f; // 750 мм в метрах
        private const int PRE_MAKEUP_DURATION_MS = 5000; // 5 секунд
        private const float MAKEUP_LENGTH_MAX = 0.200f; // 200 мм силовой навёртки
        private const int MAKEUP_DURATION_MS = 8000; // 8 секунд
        private const float MAX_TORQUE = 8000f; // Максимальный момент
        private const int UPDATE_INTERVAL_MS = 50; // Интервал обновления точек (50 мс = 20 Hz)

        // Текущие значения симуляции
        private JointResult _currentResult;
        private int _timestamp;

        #region События IJointProcessWorker

        public event EventHandler<JointResult> PipeAppear;
        public event EventHandler<EventArgs> RecordingBegun;
        public event EventHandler<JointResult> RecordingFinished;
        public event EventHandler<JointResult> AwaitForEvaluation;
        public event EventHandler<TqTnLenPoint> NewTqTnLenPoint;
        public event EventHandler<JointResult> JointFinished;

        #endregion

        #region Свойства IJointProcessWorker

        public bool CyclicallyListen { get; set; }

        #endregion

        #region Методы IJointProcessWorker

        public void Evaluate(uint result)
        {
            if (_currentResult != null)
            {
                _currentResult.ResultTotal = result;
                // Можно добавить логику оценки
            }
        }

        public void SetActualRecipe(JointRecipe recipe)
        {
            // В тестовой реализации сохраняем рецепт для будущего использования
        }

        #endregion

        #region Публичные методы управления симуляцией

        /// <summary>
        /// Запускает симуляцию процесса свинчивания
        /// </summary>
        public void Start()
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => RunSimulation(_cancellationTokenSource.Token));
        }

        /// <summary>
        /// Останавливает симуляцию
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
            {
                return;
            }

            _cancellationTokenSource?.Cancel();
            _isRunning = false;
        }

        #endregion

        #region Логика симуляции

        private async Task RunSimulation(CancellationToken cancellationToken)
        {
            try
            {
                do
                {
                    _timestamp = 0;

                    // Phase 0: Труба появилась (PipeAppear)
                    await SimulatePipeAppear(cancellationToken);

                    // Phase 1: Pre-makeup - изменение длины от 0 до 750 мм
                    await SimulatePreMakeup(cancellationToken);

                    // Phase 2: Начало записи (RecordingBegun)
                    await SimulateRecordingBegun(cancellationToken);

                    // Phase 3: Makeup - силовое свинчивание
                    await SimulateMakeup(cancellationToken);

                    // Phase 4: Запись завершена (RecordingFinished)
                    await SimulateRecordingFinished(cancellationToken);

                    // Phase 5: Свинчивание завершено (JointFinished) через 2 секунды
                    await Task.Delay(2000, cancellationToken);
                    JointFinished?.Invoke(this, _currentResult);

                } while (CyclicallyListen && !cancellationToken.IsCancellationRequested);
            }
            catch (OperationCanceledException)
            {
                // Симуляция остановлена
            }
            finally
            {
                _isRunning = false;
            }
        }

        private async Task SimulatePipeAppear(CancellationToken cancellationToken)
        {
            // Создаём тестовый JointResult с минимальными данными
            var recipe = CreateTestRecipe();
            _currentResult = new JointResult(recipe)
            {
                StartTimeStamp = DateTime.Now,
                EvaluationVerdict = new EvaluationVerdict()
            };

            PipeAppear?.Invoke(this, _currentResult);

            await Task.Delay(500, cancellationToken); // Небольшая задержка
        }

        private async Task SimulatePreMakeup(CancellationToken cancellationToken)
        {
            int elapsedMs = 0;
            float length = 0f;

            while (elapsedMs < PRE_MAKEUP_DURATION_MS && !cancellationToken.IsCancellationRequested)
            {
                // Линейное увеличение длины от 0 до 750 мм
                float progress = (float)elapsedMs / PRE_MAKEUP_DURATION_MS;
                length = PRE_MAKEUP_LENGTH_MAX * progress;

                var point = new TqTnLenPoint
                {
                    Length = length,
                    Torque = 0f,
                    Turns = 0f,
                    TurnsPerMinute = 0f,
                    TimeStamp = _timestamp
                };

                NewTqTnLenPoint?.Invoke(this, point);

                await Task.Delay(UPDATE_INTERVAL_MS, cancellationToken);
                elapsedMs += UPDATE_INTERVAL_MS;
                _timestamp += UPDATE_INTERVAL_MS;
            }

            // Сброс длины в 0
            await Task.Delay(200, cancellationToken);
            _timestamp = 0; // Сбрасываем таймштамп для фазы записи
        }

        private async Task SimulateRecordingBegun(CancellationToken cancellationToken)
        {
            _currentResult.StartTimeStamp = DateTime.Now;
            RecordingBegun?.Invoke(this, EventArgs.Empty);

            await Task.Delay(300, cancellationToken);
        }

        private async Task SimulateMakeup(CancellationToken cancellationToken)
        {
            int elapsedMs = 0;
            float baseLength = 0f;
            float baseTurns = 0f;
            float baseTorque = 0f;

            while (elapsedMs < MAKEUP_DURATION_MS && !cancellationToken.IsCancellationRequested)
            {
                float progress = (float)elapsedMs / MAKEUP_DURATION_MS;

                // Линейный рост длины
                baseLength = MAKEUP_LENGTH_MAX * progress;

                // Линейный рост оборотов (допустим, 10 оборотов за процесс)
                baseTurns = 10f * progress;

                // Линейный рост момента с шумом
                baseTorque = MAX_TORQUE * progress;
                float noise = (float)(_random.NextDouble() - 0.5) * MAX_TORQUE * 0.05f; // ±5% шум
                float torque = Math.Max(0, baseTorque + noise);

                // Обороты в минуту (допустим, ~20 RPM)
                float rpm = 15f + (float)_random.NextDouble() * 10f;

                var point = new TqTnLenPoint
                {
                    Length = baseLength,
                    Torque = torque,
                    Turns = baseTurns,
                    TurnsPerMinute = rpm,
                    TimeStamp = _timestamp
                };

                _currentResult.Series.Add(point);
                NewTqTnLenPoint?.Invoke(this, point);

                await Task.Delay(UPDATE_INTERVAL_MS, cancellationToken);
                elapsedMs += UPDATE_INTERVAL_MS;
                _timestamp += UPDATE_INTERVAL_MS;
            }

            // Момент падает до 0 (очень быстро)
            for (int i = 0; i < 5; i++)
            {
                float torque = baseTorque * (1f - (i / 5f));
                var point = new TqTnLenPoint
                {
                    Length = baseLength,
                    Torque = Math.Max(0, torque),
                    Turns = baseTurns,
                    TurnsPerMinute = 0f,
                    TimeStamp = _timestamp
                };

                _currentResult.Series.Add(point);
                NewTqTnLenPoint?.Invoke(this, point);

                await Task.Delay(50, cancellationToken);
                _timestamp += 50;
            }

            // Финальные значения
            _currentResult.FinalLength = baseLength;
            _currentResult.FinalTurns = baseTurns;
            _currentResult.FinalTorque = 0f;
        }

        private async Task SimulateRecordingFinished(CancellationToken cancellationToken)
        {
            _currentResult.FinishTimeStamp = DateTime.Now;
            RecordingFinished?.Invoke(this, _currentResult);

            await Task.Delay(100, cancellationToken);

            // Опционально можно вызвать AwaitForEvaluation
            AwaitForEvaluation?.Invoke(this, _currentResult);
        }

        private JointRecipe CreateTestRecipe()
        {
            // Создаём минимальный тестовый рецепт
            // Пользователь заполнит данные сам позже
            return new JointRecipe
            {
                Id = Guid.NewGuid(),
                Name = "Test Recipe",
                JointMode = JointMode.Torque,
                SelectedThreadType = ThreadType.RIGHT,
                PIPE_TYPE = "Test Pipe"
            };
        }

        #endregion
    }
}
