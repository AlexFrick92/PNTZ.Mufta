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
            UpdateConstantLines(recipe);
            UpdateStrips(recipe);

            //График: Момент/Обороты
            TorqueTurnsChart.YMax = recipe.MU_Tq_Max * 1.1;
            TorqueTurnsChart.XMax = 3;


            //График: Момент/Длина
            TorqueLengthChart.YMax = recipe.MU_Tq_Max * 1.1;
            TorqueLengthChart.XMax = recipe.MU_Len_Max * 1.05;

            //График: Обороты/ОборотыВминуту
            TurnsPerMinuteTurnsChart.XMax = 3;

            //График: Момент/Время
            TorqueTimeChart.YMax = recipe.MU_Tq_Max * 1.1;
            TorqueTimeChart.XMax = 15000;

        }

        private void UpdateConstantLines(JointRecipe recipe)
        {
            //Создаем постоянные прямые для графика
            var torqueMinLine = new ConstantLineViewModel(recipe.MU_Tq_Min, "Мин", Brushes.DarkRed, "F0");
            var torqueMaxLine = new ConstantLineViewModel(recipe.MU_Tq_Max, "Макс", Brushes.DarkRed, "F0");
            var torqueOptLine = new ConstantLineViewModel(recipe.MU_Tq_Opt, "Опт", Brushes.DarkGreen, "F0");
            var torqueDump = new ConstantLineViewModel(recipe.MU_Tq_Dump, "сброс", Brushes.DarkGray, "F0");
            var lengthMinLine = new ConstantLineViewModel(recipe.MU_Len_Min, "Мин", Brushes.DarkRed, "F0");
            var lengthMaxLine = new ConstantLineViewModel(recipe.MU_Len_Max, "Макс", Brushes.DarkRed, "F0");
            var lengthDump = new ConstantLineViewModel(recipe.MU_Len_Dump, "сброс", Brushes.DarkGray, "F0");
            var shoulderMinLine = new ConstantLineViewModel(recipe.MU_TqShoulder_Min, "Мин. буртик", Brushes.DarkOrange, "F0");
            var shoulderMaxLine = new ConstantLineViewModel(recipe.MU_TqShoulder_Max, "Макс. буртик", Brushes.DarkOrange, "F0");

            TorqueTurnsChart.YConstantLines.Clear();
            TorqueTurnsChart.XConstantLines.Clear();
            TorqueLengthChart.YConstantLines.Clear();
            TorqueLengthChart.XConstantLines.Clear();
            TurnsPerMinuteTurnsChart.YConstantLines.Clear();
            TurnsPerMinuteTurnsChart.XConstantLines.Clear();
            TorqueTimeChart.YConstantLines.Clear();
            TorqueTimeChart.XConstantLines.Clear();

            switch (recipe.JointMode)
            {
                case JointMode.Torque:
                    TorqueTurnsChart.YConstantLines.AddRange(torqueMinLine, torqueMaxLine, torqueOptLine, torqueDump);
                    TorqueLengthChart.YConstantLines.AddRange(torqueMinLine, torqueMaxLine);
                    break;
                case JointMode.TorqueShoulder:
                    TorqueTurnsChart.YConstantLines.AddRange(torqueMinLine, torqueMaxLine, shoulderMinLine, shoulderMaxLine, torqueDump, torqueOptLine);
                    TorqueLengthChart.YConstantLines.AddRange(torqueMinLine, torqueMaxLine, torqueDump);
                    break;

                case JointMode.TorqueLength:
                    TorqueLengthChart.YConstantLines.AddRange(torqueMinLine, torqueMaxLine, torqueDump);
                    TorqueTurnsChart.YConstantLines.AddRange(torqueMinLine, torqueMaxLine, torqueOptLine, torqueDump);
                    TorqueLengthChart.XConstantLines.AddRange(lengthMinLine, lengthMaxLine, lengthDump);
                    break;
                case JointMode.Length:
                    TorqueLengthChart.XConstantLines.AddRange(lengthMinLine, lengthMaxLine, lengthDump);
                    break;

                case JointMode.TorqueJVal:
                case JointMode.Jval:
                    break;
            }
        }

        private void UpdateStrips(JointRecipe recipe)
        {
            // Создаем выделенные области для допустимых диапазонов
            var torqueStrip = new StripViewModel(
                recipe.MU_Tq_Min,
                recipe.MU_Tq_Max,
                new SolidColorBrush(Colors.LightGreen) { Opacity = 0.2 });

            var lengthStrip = new StripViewModel(
                recipe.MU_Len_Min,
                recipe.MU_Len_Max,
                new SolidColorBrush(Colors.LightGreen) { Opacity = 0.2 });

            var shoulderStrip = new StripViewModel(
                recipe.MU_TqShoulder_Min,
                recipe.MU_TqShoulder_Max,
                new SolidColorBrush(Colors.OrangeRed) { Opacity = 0.2 });

            // Очищаем существующие Strip'ы
            TorqueTurnsChart.YStrips.Clear();
            TorqueTurnsChart.XStrips.Clear();
            TorqueLengthChart.YStrips.Clear();
            TorqueLengthChart.XStrips.Clear();
            TurnsPerMinuteTurnsChart.YStrips.Clear();
            TurnsPerMinuteTurnsChart.XStrips.Clear();
            TorqueTimeChart.YStrips.Clear();
            TorqueTimeChart.XStrips.Clear();

            // Добавляем Strip'ы в зависимости от режима
            switch (recipe.JointMode)
            {
                case JointMode.Torque:
                    TorqueTurnsChart.YStrips.Add(torqueStrip);
                    TorqueLengthChart.YStrips.Add(torqueStrip);                    
                    break;

                case JointMode.TorqueShoulder:
                    TorqueTurnsChart.YStrips.AddRange(torqueStrip, shoulderStrip);
                    TorqueLengthChart.YStrips.Add(torqueStrip);                    
                    break;

                case JointMode.TorqueLength:
                    TorqueTurnsChart.YStrips.Add(torqueStrip);
                    TorqueLengthChart.YStrips.Add(torqueStrip);
                    TorqueLengthChart.XStrips.Add(lengthStrip);                    
                    break;

                case JointMode.Length:
                    TorqueLengthChart.XStrips.Add(lengthStrip);
                    break;

                case JointMode.TorqueJVal:
                case JointMode.Jval:
                    break;
            }
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
