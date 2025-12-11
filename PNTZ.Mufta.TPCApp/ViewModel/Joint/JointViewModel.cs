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
        /// <summary>
        /// Обработчик новой точки данных из фонового потока
        /// </summary>
        private void OnNewTqTnLenPoint(object sender, TqTnLenPoint point)
        {
            JointProcessDataViewModel.ActualPoint = point;
        }

        /// <summary>
        /// Обработчик появления трубы из фонового потока
        /// </summary>
        private void OnPipeAppear(object sender, JointResult result)
        {
            JointProcessChartViewModel.SetMvsData(result);
        }

        /// <summary>
        /// Обработчик начала записи из фонового потока
        /// </summary>
        private void OnRecordingBegun(object sender, EventArgs e)
        {
            _jointProcessWorker.NewTqTnLenPoint += AddPointToChart;
            JointProcessDataViewModel.BeginNewJointing();
        }

        private void AddPointToChart(object sender, TqTnLenPoint e)
        {
            JointProcessChartViewModel.TqTnLenPoints.Add(e);
        }

        /// <summary>
        /// Обработчик завершения записи из фонового потока
        /// </summary>
        private void OnRecordingFinished(object sender, JointResult result)
        {
            _jointProcessWorker.NewTqTnLenPoint -= AddPointToChart;
            JointProcessDataViewModel.FinishJointing(result);
        }
        private void OnJointFinished(object sender, JointResult r)
        {
            JointProcessChartViewModel.FinishJointing(r);
            JointProcessDataViewModel.FinishJointing(r);
        }
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
