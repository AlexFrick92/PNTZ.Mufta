using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            //Обновление показаний датчиков
            _jointProcessWorker.NewTqTnLenPoint += (s, p) => JointProcessChartViewModel.TqTnLenPoints.Add(p);
            _jointProcessWorker.NewTqTnLenPoint += (s, p) => JointProcessDataViewModel.ActualPoint = p;
            //Появление трубы на позиции
            _jointProcessWorker.PipeAppear += (s, r) => JointProcessChartViewModel.SetMvsData(r);            
            //Свинчивание начато
            _jointProcessWorker.RecordingBegun += (s, e) => JointProcessDataViewModel.BeginNewJointing();
            //Свинчивание завершено
            _jointProcessWorker.RecordingFinished += (s, r) => JointProcessDataViewModel.FinishJointing(r);
            //Труба прошла оценку. Процесс завершен
            _jointProcessWorker.JointFinished += (s, r) => JointProcessChartViewModel.FinishJointing(r);
            _jointProcessWorker.JointFinished += (s, r) => JointProcessDataViewModel.FinishJointing(r);
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
