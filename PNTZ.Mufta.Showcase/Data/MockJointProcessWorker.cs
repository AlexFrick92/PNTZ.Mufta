using System;
using System.Diagnostics;
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
        private const float MAX_TORQUE = 8000f; // Максимальный момент по умолчанию

        /// <summary>
        /// Интервал обновления точек в миллисекундах (настраивается извне)
        /// </summary>
        public int UpdateIntervalMs { get; set; } = 50; // По умолчанию 50 мс = 20 Hz

        // Текущие значения симуляции
        private JointResult _currentResult;     
        private JointRecipe _currentRecipe;
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
            _currentRecipe = recipe;
        }

        public void Initialize()
        {
            Task.Run(() =>
            {
                var point = new TqTnLenPoint
                {
                    Length = 0f,
                    Torque = 0f,
                    Turns = 0f,
                    TurnsPerMinute = 0f,
                    TimeStamp = _timestamp
                };

                NewTqTnLenPoint?.Invoke(this, point);
            });
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
                _timestamp = 0;

                // Phase 0: Труба появилась (PipeAppear)
                await SimulatePipeAppear(cancellationToken);
                Debug.WriteLine("Pipe appeared.");

                // Phase 1: Pre-makeup - изменение длины от 0 до 750 мм
                await SimulatePreMakeup(cancellationToken);
                Debug.WriteLine("Pre-makeup phase completed.");

                // Phase 2: Начало записи (RecordingBegun)
                await SimulateRecordingBegun(cancellationToken);
                Debug.WriteLine("Recording begun.");

                // Phase 3: Makeup - силовое свинчивание
                await SimulateMakeup(cancellationToken);
                Debug.WriteLine("Makeup phase completed.");

                // Phase 4: Запись завершена (RecordingFinished)
                await SimulateRecordingFinished(cancellationToken);
                Debug.WriteLine("Recording finished.");


                // Phase 5: Свинчивание завершено (JointFinished) через 2 секунды
                await Task.Delay(2000, cancellationToken);
                Debug.WriteLine("Joint finished."); 


                _currentResult.EvaluationVerdict = new EvaluationVerdict
                {
                    TorqueOk = true,
                    LentghOk = true,
                    ShoulderOk = true
                };
                _currentResult.ResultTotal = 1; // Годная
                JointFinished?.Invoke(this, _currentResult);

            }            
            catch (OperationCanceledException)
            {
                // Симуляция остановлена
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in simulation: {ex.Message}");
            }   
            finally
            {
                _isRunning = false;
            }
        }

        private async Task SimulatePipeAppear(CancellationToken cancellationToken)
        {
            // Создаём тестовый JointResult с минимальными данными            
            _currentResult = new JointResult(_currentRecipe)
            {
                StartTimeStamp = DateTime.Now,
                MVS_Len = 85f
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

                await Task.Delay(UpdateIntervalMs, cancellationToken);
                elapsedMs += UpdateIntervalMs;
                _timestamp += UpdateIntervalMs;
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

            // Получаем пределы из рецепта или используем значения по умолчанию
            float targetLength = _currentRecipe?.MU_Len_Dump ?? MAKEUP_LENGTH_MAX;
            float targetTorque = _currentRecipe?.MU_Tq_Dump ?? MAX_TORQUE;
            float targetTurns = 10f;

            while (elapsedMs < MAKEUP_DURATION_MS && !cancellationToken.IsCancellationRequested)
            {
                float progress = (float)elapsedMs / MAKEUP_DURATION_MS;

                // Линейный рост длины
                baseLength = targetLength * progress;

                // Линейный рост оборотов
                baseTurns = targetTurns * progress;

                // Линейный рост момента с шумом
                baseTorque = targetTorque * progress;
                float noise = (float)(_random.NextDouble() - 0.5) * targetTorque * 0.05f; // ±5% шум
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

                await Task.Delay(UpdateIntervalMs, cancellationToken);
                elapsedMs += UpdateIntervalMs;
                _timestamp += UpdateIntervalMs;
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

                await Task.Delay(UpdateIntervalMs, cancellationToken);
                _timestamp += UpdateIntervalMs;
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

        #endregion
    }
}
