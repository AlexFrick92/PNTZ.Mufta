using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
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
        public JointViewModel(IJointProcessWorker jointProcessWorker, IRecipeLoader recipeLoader, ILogger logger, ICliProgram cliProgram)
        {
            _jointProcessWorker = jointProcessWorker;
            _recipeLoader = recipeLoader;
            _logger = logger;
            _cliProgram = cliProgram;
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
        private ICliProgram _cliProgram;
    }
}
