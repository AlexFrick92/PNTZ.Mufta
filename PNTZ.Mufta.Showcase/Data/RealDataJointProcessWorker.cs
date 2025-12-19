using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;

namespace PNTZ.Mufta.Showcase.Data
{
    /// <summary>
    /// Воспроизводит реальные данные из JointResult для тестирования и отладки
    /// </summary>
    public class RealDataJointProcessWorker : IJointProcessTableWorker
    {
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning;
        private JointResultTable _realData;

        /// <summary>
        /// Интервал между точками данных в миллисекундах
        /// </summary>
        public int UpdateIntervalMs { get; set; } = 50;

        #region События IJointProcessWorker

        public event EventHandler<JointResultTable> PipeAppear;
        public event EventHandler<EventArgs> RecordingBegun;
        public event EventHandler<JointResultTable> RecordingFinished;
        public event EventHandler<JointResultTable> AwaitForEvaluation;
        public event EventHandler<TqTnLenPoint> NewTqTnLenPoint;
        public event EventHandler<JointResultTable> JointFinished;

        #endregion

        #region Свойства IJointProcessWorker

        public bool CyclicallyListen { get; set; }

        #endregion

        #region Методы IJointProcessWorker

        public void Evaluate(uint result)
        {
            if (_realData != null)
            {
                _realData.ResultTotal = result;
            }
        }

        public void SetActualRecipe(JointRecipeTable recipe)
        {
            // В режиме воспроизведения реальных данных рецепт уже загружен из базы
        }

        #endregion

        #region Публичные методы

        /// <summary>
        /// Загрузить реальные данные для воспроизведения
        /// </summary>
        /// <param name="realData">JointResult с реальными данными</param>
        public void LoadRealData(JointResultTable realData)
        {
            if (realData == null)
                throw new ArgumentNullException(nameof(realData));

            if (realData.Series == null || realData.PointSeries.Count == 0)
                throw new ArgumentException("JointResult не содержит точек данных (Series пуст)", nameof(realData));

            _realData = realData;
            Debug.WriteLine($"Loaded real data: {realData.Recipe?.Name}, {realData.PointSeries.Count} points");
        }

        /// <summary>
        /// Запускает воспроизведение реальных данных
        /// </summary>
        public void Start()
        {
            if (_isRunning)
            {
                Debug.WriteLine("Already running");
                return;
            }

            if (_realData == null)
            {
                Debug.WriteLine("No real data loaded");
                return;
            }

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => RunPlayback(_cancellationTokenSource.Token));
        }

        /// <summary>
        /// Останавливает воспроизведение
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

        #region Логика воспроизведения

        private async Task RunPlayback(CancellationToken cancellationToken)
        {
            try
            {
                Debug.WriteLine("=== Playback started ===");

                // Phase 0: Труба появилась (PipeAppear)
                await SimulatePipeAppear(cancellationToken);
                Debug.WriteLine("Pipe appeared.");

                await Task.Delay(500, cancellationToken);

                // Phase 1: Начало записи (RecordingBegun)
                await SimulateRecordingBegun(cancellationToken);
                Debug.WriteLine("Recording begun.");

                await Task.Delay(300, cancellationToken);

                // Phase 2: Воспроизведение реальных точек данных
                await PlaybackRealDataPoints(cancellationToken);
                Debug.WriteLine($"Playback completed. {_realData.PointSeries.Count} points played.");

                // Phase 3: Запись завершена (RecordingFinished)
                await SimulateRecordingFinished(cancellationToken);
                Debug.WriteLine("Recording finished.");

                // Phase 4: Свинчивание завершено (JointFinished) через 2 секунды
                await Task.Delay(2000, cancellationToken);
                JointFinished?.Invoke(this, _realData);
                Debug.WriteLine("Joint finished.");
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Playback cancelled");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in playback: {ex.Message}");
            }
            finally
            {
                _isRunning = false;
            }
        }

        private async Task SimulatePipeAppear(CancellationToken cancellationToken)
        {
            PipeAppear?.Invoke(this, _realData);
            await Task.Delay(100, cancellationToken);
        }

        private async Task SimulateRecordingBegun(CancellationToken cancellationToken)
        {
            RecordingBegun?.Invoke(this, EventArgs.Empty);
            await Task.Delay(100, cancellationToken);
        }

        private async Task PlaybackRealDataPoints(CancellationToken cancellationToken)
        {
            foreach (var point in _realData.PointSeries)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                NewTqTnLenPoint?.Invoke(this, point);

                await Task.Delay(UpdateIntervalMs, cancellationToken);
            }
        }

        private async Task SimulateRecordingFinished(CancellationToken cancellationToken)
        {
            RecordingFinished?.Invoke(this, _realData);
            await Task.Delay(100, cancellationToken);

            // Опционально можно вызвать AwaitForEvaluation
            AwaitForEvaluation?.Invoke(this, _realData);
        }

        #endregion
    }
}
