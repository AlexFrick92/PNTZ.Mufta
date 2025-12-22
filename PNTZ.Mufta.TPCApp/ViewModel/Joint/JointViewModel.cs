using System;
using System.Windows.Input;
using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
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
        public JointViewModel(IJointProcessTableWorker jointProcessWorker, IRecipeTableLoader recipeLoader, ILogger logger)
        {
            _jointProcessWorker = jointProcessWorker ?? throw new ArgumentNullException(nameof(jointProcessWorker));
            _recipeLoader = recipeLoader ?? throw new ArgumentNullException(nameof(_recipeLoader));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            //Подписываемся на события загрузчика рецептов
            _recipeLoader.RecipeLoaded += OnRecipeLoaded;            

            //Подписываемся на события процесса навёртки
            //Обновление показаний датчиков - складываем в очередь для обработки в UI-потоке
            _jointProcessWorker.NewTqTnLenPoint += OnNewTqTnLenPoint;
            //Появление трубы на позиции - маршалим в UI-поток
            _jointProcessWorker.PipeAppear += OnPipeAppear;
            //Свинчивание начато - включаем запись точек
            _jointProcessWorker.RecordingBegun += OnRecordingBegun;
            //Свинчивание завершено - останавливаем запись точек
            _jointProcessWorker.RecordingFinished += OnRecordingFinished;
            //Требуется оценка
            _jointProcessWorker.AwaitForEvaluation += OnAwaitForEvaluation;
            //Труба прошла оценку. Процесс завершен
            _jointProcessWorker.JointFinished += OnJointFinished;

            _jointProcessWorker.CyclicallyListen = true;

            //Кнопки установки результата
            SetGoodResultCommand = new RelayCommand(SetGoodResult);
            SetBadResultCommand = new RelayCommand(SetBadResult);
        }
        /// <summary>
        /// Графики
        /// </summary>
        public JointProcessChartViewModel JointProcessChartViewModel { get; set; } = new JointProcessChartViewModel();
        /// <summary>
        /// Панель данных слева
        /// </summary>
        public JointProcessDataViewModel JointProcessDataViewModel { get; set; } = new JointProcessDataViewModel();
        /// <summary>
        /// Видимость окна оценки
        /// </summary>
        public bool EvaluationVisible { get => _evaluationVisible; set { _evaluationVisible = value; OnPropertyChanged(nameof(EvaluationVisible)); } }
        /// <summary>
        /// Установить оценку "годная"
        /// </summary>
        public ICommand SetGoodResultCommand { get; private set; }
        /// <summary>
        /// Установить оценку "брак"
        /// </summary>
        public ICommand SetBadResultCommand { get; private set; }
        //Приватные поля
        private IJointProcessTableWorker _jointProcessWorker;
        private IRecipeTableLoader _recipeLoader;
        private ILogger _logger;
        private bool _evaluationVisible = false;
        /// Обработчик успешной загрузки рецепта
        private void OnRecipeLoaded(object sender, JointRecipeTable recipe)
        {
            if (recipe == null)
                return;

            // Обновляем рецепт в дочерних ViewModels
            JointProcessDataViewModel.UpdateRecipe(recipe);
            JointProcessChartViewModel.UpdateRecipe(recipe);
            _jointProcessWorker.SetActualRecipe(recipe);

            _logger?.Info($"Рецепт загружен: {recipe.Name} (Режим: {recipe.JointMode})");
        }                
        /// Обработчик новой точки данных из фонового потока
        private void OnNewTqTnLenPoint(object sender, TqTnLenPoint point) => JointProcessDataViewModel.ActualPoint = point;
        /// Обработчик появления трубы из фонового потока
        private void OnPipeAppear(object sender, JointResultTable result)
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
        private void AddPointToChart(object sender, TqTnLenPoint e) => JointProcessChartViewModel.TqTnLenPointsQueue.Enqueue(e);
        /// Обработчик завершения записи из фонового потока
        private void OnRecordingFinished(object sender, JointResultTable result)
        {
            _jointProcessWorker.NewTqTnLenPoint -= AddPointToChart;
            JointProcessDataViewModel.FinishJointing(result);
            JointProcessChartViewModel.RecordingStop();
        }
        /// Оценка оператором
        private void OnAwaitForEvaluation(object sender, JointResultTable e)
        {
            JointProcessChartViewModel.AutoEvaluationResult(e);
            EvaluationVisible = true;
        }
        private void SetGoodResult(object arg)
        {
            _jointProcessWorker.Evaluate(1);
            EvaluationVisible = false;
        }
        private void SetBadResult(object arg)
        {
            _jointProcessWorker.Evaluate(2);
            EvaluationVisible = false;
        }        
        private void OnJointFinished(object sender, JointResultTable r)
        {
            JointProcessChartViewModel.FinishJointing(r);
            JointProcessDataViewModel.FinishJointing(r);
        }              
    }
}
