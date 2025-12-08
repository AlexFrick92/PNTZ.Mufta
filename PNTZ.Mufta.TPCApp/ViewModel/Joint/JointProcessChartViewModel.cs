using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.ViewModel.Control;
using Promatis.Core.Extensions;
using System.Windows.Media;

namespace PNTZ.Mufta.TPCApp.ViewModel.Joint
{
    /// <summary>
    /// ViewModel для JointProcessChartView.
    /// Управляет 4 графиками процесса муфтонавёртки.
    /// </summary>
    public class JointProcessChartViewModel : BaseViewModel
    {
        /// <summary>
        /// График: Момент/обороты
        /// </summary>
        public ChartViewModel TorqueTurnsChart { get; private set; }

        /// <summary>
        /// График: (Обороты/Мин)/обороты
        /// </summary>
        public ChartViewModel TurnsPerMinuteTurnsChart { get; private set; }

        /// <summary>
        /// График: Момент/длина
        /// </summary>
        public ChartViewModel TorqueLengthChart { get; private set; }

        /// <summary>
        /// График: Момент/время
        /// </summary>
        public ChartViewModel TorqueTimeChart { get; private set; }

        public JointProcessChartViewModel()
        {
            InitializeCharts();
        }
        /// <summary>
        /// Настроить графики под рецепт
        /// </summary>
        /// <param name="recipe"></param>
        public void UpdateRecipe(JointRecipe recipe)
        {
            var torqueMaxLine = new ConstantLineViewModel(recipe.MU_Tq_Max, "Макс", Brushes.DarkRed, "F0");
            var torqueMinLine = new ConstantLineViewModel(recipe.MU_Tq_Min, "Мин", Brushes.DarkRed, "F0");

            //График: Момент/Обороты
            TorqueTurnsChart.YMax = recipe.MU_Tq_Max * 1.1;
            TorqueTurnsChart.XMax = 3;
            TorqueTurnsChart.YConstantLines.Clear();
            TorqueTurnsChart.YConstantLines.AddRange(                
                torqueMaxLine,
                torqueMinLine
            );


            //График: Момент/Длина
            TorqueLengthChart.YMax = recipe.MU_Tq_Max * 1.1;
            TorqueLengthChart.XMax = recipe.MU_Len_Max * 1.05;

            //График: Обороты/ОборотыВминуту
            TurnsPerMinuteTurnsChart.XMax = 3;

            //График: Момент/Время
            TorqueTimeChart.YMax = recipe.MU_Tq_Max * 1.1;
            TorqueTimeChart.XMax = 15000;

        }

        private void InitializeCharts()
        {
            // График: Момент/обороты
            TorqueTurnsChart = new ChartViewModel
            {
                ArgumentMember = "Turns",
                XAxisTitle = "Обороты",
                YAxisTitle = "Момент",
            };

            // График: (Обороты/Мин)/обороты
            TurnsPerMinuteTurnsChart = new ChartViewModel
            {
                ArgumentMember = "Turns",
                XAxisTitle = "Обороты",
                YAxisTitle = "Обороты/Мин",
            };

            // График: Момент/длина
            TorqueLengthChart = new ChartViewModel
            {
                ArgumentMember = "Length",
                XAxisTitle = "Длина",
                YAxisTitle = "Момент",
            };

            // График: Момент/время
            TorqueTimeChart = new ChartViewModel
            {
                ArgumentMember = "TimeStamp",
                XAxisTitle = "Время",
                YAxisTitle = "Момент",
            };

            // Уведомляем View о готовности всех графиков
            OnPropertyChanged(nameof(TorqueTurnsChart));
            OnPropertyChanged(nameof(TurnsPerMinuteTurnsChart));
            OnPropertyChanged(nameof(TorqueLengthChart));
            OnPropertyChanged(nameof(TorqueTimeChart));
        }

        
    }
}
