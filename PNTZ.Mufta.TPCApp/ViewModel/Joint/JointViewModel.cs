using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using Promatis.Core.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Toolkit.IO;

namespace PNTZ.Mufta.TPCApp.ViewModel.Joint
{
    /// <summary>
    /// Процесс свинчивания
    /// </summary>
    public class JointViewModel : BaseViewModel
    {
        /// <summary>
        /// Конструктор для production использования
        /// </summary>
        public JointViewModel(IJointProcessWorker jointProcessWorker, IRecipeLoader recipeLoader, ILogger logger)
        {
            _jointProcessWorker = jointProcessWorker ?? throw new ArgumentNullException(nameof(jointProcessWorker));
            _recipeLoader = recipeLoader ?? throw new ArgumentNullException(nameof(_recipeLoader));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Инициализация таймера для обновления UI из фоновых потоков
            InitializeUiUpdateTimer();

            //Подписываемся на события загрузчика рецептов
            _recipeLoader.RecipeLoaded += OnRecipeLoaded;
            _recipeLoader.RecipeLoadFailed += OnRecipeLoadFailed;

            //Подписываемся на события процесса навёртки
            //Обновление показаний датчиков - складываем в очередь для обработки в UI-потоке
            _jointProcessWorker.NewTqTnLenPoint += OnNewTqTnLenPoint;
            //Появление трубы на позиции - маршалим в UI-поток
            _jointProcessWorker.PipeAppear += OnPipeAppear;
            //Свинчивание начато - включаем запись точек
            _jointProcessWorker.RecordingBegun += OnRecordingBegun;
            //Свинчивание завершено - останавливаем запись точек
            _jointProcessWorker.RecordingFinished += OnRecordingFinished;
            //Труба прошла оценку. Процесс завершен
            _jointProcessWorker.JointFinished += OnJointFinished;
        }

        /// <summary>
        /// Графики
        /// </summary>
        public JointProcessChartViewModel JointProcessChartViewModel { get; set; } = new JointProcessChartViewModel();
        /// <summary>
        /// Панель данных слева
        /// </summary>
        public JointProcessDataViewModel JointProcessDataViewModel { get; set; } = new JointProcessDataViewModel();
        //Приватные поля
        private IJointProcessWorker _jointProcessWorker;
        private IRecipeLoader _recipeLoader;
        private ILogger _logger;

        // Буферизация точек для thread-safe добавления в UI-поток
        private ConcurrentQueue<TqTnLenPoint> _pointsQueue = new ConcurrentQueue<TqTnLenPoint>();
        private DispatcherTimer _uiUpdateTimer;
        private volatile bool _isRecording = false; // Флаг активной записи точек
        private TqTnLenPoint _lastPoint = null;

        #region Инициализация и обработка фоновых событий

        /// <summary>
        /// Инициализация таймера для обновления UI из фонового потока
        /// </summary>
        private void InitializeUiUpdateTimer()
        {
            _uiUpdateTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(30) // 30 мс = ~33 обновления в секунду
            };
            _uiUpdateTimer.Tick += UiUpdateTimer_Tick;
            _uiUpdateTimer.Start();
        }

        /// <summary>
        /// Обработчик новой точки данных из фонового потока
        /// </summary>
        private void OnNewTqTnLenPoint(object sender, TqTnLenPoint point)
        {
            JointProcessDataViewModel.ActualPoint = point;

            // Складываем точку в очередь только если идёт запись
            if (_isRecording)
            {
                _pointsQueue.Enqueue(point);
                _lastPoint = point;
            }
        }

        /// <summary>
        /// Обработчик появления трубы из фонового потока
        /// </summary>
        private void OnPipeAppear(object sender, JointResult result)
        {
            // Маршалим в UI-поток
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                // Очищаем очередь точек при появлении новой трубы
                while (_pointsQueue.TryDequeue(out _)) { }

                JointProcessChartViewModel.SetMvsData(result);
            }));
        }

        /// <summary>
        /// Обработчик начала записи из фонового потока
        /// </summary>
        private void OnRecordingBegun(object sender, EventArgs e)
        {
            // Включаем запись точек
            _isRecording = true;

            JointProcessDataViewModel.BeginNewJointing();
        }

        /// <summary>
        /// Обработчик завершения записи из фонового потока
        /// </summary>
        private void OnRecordingFinished(object sender, JointResult result)
        {
            // Останавливаем запись точек
            _isRecording = false;
            JointProcessDataViewModel.FinishJointing(result);

        }
        private void OnJointFinished(object sender, JointResult r)
        {
            JointProcessChartViewModel.FinishJointing(r);
            JointProcessDataViewModel.FinishJointing(r);
        }

        /// <summary>
        /// Тик таймера - извлекаем точки из очереди и добавляем в UI
        /// </summary>
        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            if(_isRecording && _lastPoint != null)
                JointProcessChartViewModel.TqTnLenPoints.Add(_lastPoint);
            return;            

            // Извлекаем все накопленные точки
            while (_pointsQueue.TryDequeue(out var point))
            {
                // Обновляем график
                JointProcessChartViewModel.TqTnLenPoints.Add(point);
                // Обновляем текущие показания
                JointProcessDataViewModel.ActualPoint = point;
            }
        }

        #endregion

        /// <summary>
        /// Обработчик успешной загрузки рецепта
        /// </summary>
        private void OnRecipeLoaded(object sender, JointRecipe recipe)
        {
            if (recipe == null)
                return;

            // Обновляем рецепт в дочерних ViewModels
            JointProcessDataViewModel.UpdateRecipe(recipe);
            JointProcessChartViewModel.UpdateRecipe(recipe);

            _logger?.Info($"Рецепт загружен: {recipe.Name} (Режим: {recipe.JointMode})");
        }

        /// <summary>
        /// Обработчик ошибки загрузки рецепта
        /// </summary>
        private void OnRecipeLoadFailed(object sender, JointRecipe recipe)
        {
            _logger?.Error($"Ошибка загрузки рецепта");
        }        
    }
}
