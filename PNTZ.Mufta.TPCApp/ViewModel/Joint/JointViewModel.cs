using System;

using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using Promatis.Core.Logging;

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
        /// Обработчик новой точки данных из фонового потока
        private void OnNewTqTnLenPoint(object sender, TqTnLenPoint point)
        {
            JointProcessDataViewModel.ActualPoint = point;
        }
        /// Обработчик появления трубы из фонового потока
        private void OnPipeAppear(object sender, JointResult result)
        {
            JointProcessChartViewModel.PipeAppear(result);
            JointProcessDataViewModel.PipeAppear();
        }
        /// Обработчик начала записи из фонового потока
        private void OnRecordingBegun(object sender, EventArgs e)
        {
            _jointProcessWorker.NewTqTnLenPoint += AddPointToChart;
            JointProcessDataViewModel.BeginNewJointing();
            JointProcessChartViewModel.RecordingBegin();
        }
        private void AddPointToChart(object sender, TqTnLenPoint e)
        {
            JointProcessChartViewModel.TqTnLenPointsQueue.Enqueue(e);
        }
        /// Обработчик завершения записи из фонового потока
        private void OnRecordingFinished(object sender, JointResult result)
        {
            _jointProcessWorker.NewTqTnLenPoint -= AddPointToChart;
            JointProcessDataViewModel.FinishJointing(result);
            JointProcessChartViewModel.RecordingStop();
        }
        private void OnJointFinished(object sender, JointResult r)
        {
            JointProcessChartViewModel.FinishJointing(r);
            JointProcessDataViewModel.FinishJointing(r);
        }
        /// Обработчик успешной загрузки рецепта
        private void OnRecipeLoaded(object sender, JointRecipe recipe)
        {
            if (recipe == null)
                return;

            // Обновляем рецепт в дочерних ViewModels
            JointProcessDataViewModel.UpdateRecipe(recipe);
            JointProcessChartViewModel.UpdateRecipe(recipe);

            _logger?.Info($"Рецепт загружен: {recipe.Name} (Режим: {recipe.JointMode})");
        }
        /// Обработчик ошибки загрузки рецепта
        private void OnRecipeLoadFailed(object sender, JointRecipe recipe)
        {
            _logger?.Error($"Ошибка загрузки рецепта");
        }        
    }
}
