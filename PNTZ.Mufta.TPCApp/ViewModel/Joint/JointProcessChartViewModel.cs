using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Styles;
using PNTZ.Mufta.TPCApp.ViewModel.Control;
using Promatis.Core.Extensions;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
            TqTnLenPoints = new ObservableCollection<TqTnLenPoint>();
            TqTnLenPoints.CollectionChanged += OnTqTnLenPointsChanged;
            InitializeCharts();
        }

        public ObservableCollection<TqTnLenPoint> TqTnLenPoints { get; private set; }

        /// <summary>
        /// Первичная настройка графиков
        /// </summary>
        private void InitializeCharts()
        {
            // График: Момент/обороты
            TorqueTurnsChart = new ChartViewModel
            {
                ArgumentMember = "Turns",
                XAxisTitle = "Обороты",
                YAxisTitle = "Момент",
            };
            TorqueTurnsChart.Series.Add(new ChartSeriesViewModel(
                "Torque",
                "Момент",
                AppColors.ChartTorqueTurns_Line as SolidColorBrush ?? Brushes.Blue,
                2.0));

            // График: (Обороты/Мин)/обороты
            TurnsPerMinuteTurnsChart = new ChartViewModel
            {
                ArgumentMember = "Turns",
                XAxisTitle = "Обороты",
                YAxisTitle = "Обороты/Мин",
            };
            TurnsPerMinuteTurnsChart.Series.Add(new ChartSeriesViewModel(
                "TurnsPerMinute",
                "Об/Мин",
                AppColors.ChartTurnsPerMinute_Line as SolidColorBrush ?? Brushes.Green,
                2.0));

            // График: Момент/длина
            TorqueLengthChart = new ChartViewModel
            {
                ArgumentMember = "Length",
                XAxisTitle = "Длина",
                YAxisTitle = "Момент",
            };
            TorqueLengthChart.Series.Add(new ChartSeriesViewModel(
                "Torque",
                "Момент",
                AppColors.ChartTorqueLength_Line as SolidColorBrush ?? Brushes.Red,
                2.0));

            // График: Момент/время
            TorqueTimeChart = new ChartViewModel
            {
                ArgumentMember = "TimeStamp",
                XAxisTitle = "Время",
                YAxisTitle = "Момент",
            };
            TorqueTimeChart.Series.Add(new ChartSeriesViewModel(
                "Torque",
                "Момент",
                AppColors.ChartTorqueTime_Line as SolidColorBrush ?? Brushes.Purple,
                2.0));

            // Уведомляем View о готовности всех графиков
            OnPropertyChanged(nameof(TorqueTurnsChart));
            OnPropertyChanged(nameof(TurnsPerMinuteTurnsChart));
            OnPropertyChanged(nameof(TorqueLengthChart));
            OnPropertyChanged(nameof(TorqueTimeChart));
        }
        #region Настройка графиков по данным рецепта       
        /// <summary>
        /// Настроить графики под рецепт
        /// </summary>
        /// <param name="recipe"></param>
        public void UpdateRecipe(JointRecipe recipe)
        {
            UpdateRanges(recipe);
            UpdateConstantLines(recipe);
            UpdateStrips(recipe);
        }
        /// <summary>
        /// Задать диапазон графиков по данным рцепта
        /// </summary>
        /// <param name="recipe"></param>
        private void UpdateRanges(JointRecipe recipe)
        {
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
        /// <summary>
        /// Нарисовать прямые линии по данным рецепта
        /// </summary>
        /// <param name="recipe"></param>
        private void UpdateConstantLines(JointRecipe recipe)
        {
            //Создаем постоянные прямые для графика
            var torqueMinLine = new ConstantLineViewModel(recipe.MU_Tq_Min, "Мин", AppColors.ChartLimitMin_Line, AppColors.ChartLimitMin_Label, "F0")
            {
                FontSize = AppFonts.ChartLimitMin_FontSize,
                FontWeight = AppFonts.ChartLimitMin_FontWeight,
                FontFamily = AppFonts.ChartLimitMin_FontFamily
            };
            var torqueMaxLine = new ConstantLineViewModel(recipe.MU_Tq_Max, "Макс", AppColors.ChartLimitMax_Line, AppColors.ChartLimitMax_Label, "F0")
            {
                FontSize = AppFonts.ChartLimitMax_FontSize,
                FontWeight = AppFonts.ChartLimitMax_FontWeight,
                FontFamily = AppFonts.ChartLimitMax_FontFamily
            };
            var torqueOptLine = new ConstantLineViewModel(recipe.MU_Tq_Opt, "Опт", AppColors.ChartLimitOptimal_Line, AppColors.ChartLimitOptimal_Label, "F0")
            {
                FontSize = AppFonts.ChartLimitOptimal_FontSize,
                FontWeight = AppFonts.ChartLimitOptimal_FontWeight,
                FontFamily = AppFonts.ChartLimitOptimal_FontFamily
            };
            var torqueDump = new ConstantLineViewModel(recipe.MU_Tq_Dump, "сброс", AppColors.ChartLimitDump_Line, AppColors.ChartLimitDump_Label, "F0")
            {
                FontSize = AppFonts.ChartLimitDump_FontSize,
                FontWeight = AppFonts.ChartLimitDump_FontWeight,
                FontFamily = AppFonts.ChartLimitDump_FontFamily
            };
            var lengthMinLine = new ConstantLineViewModel(recipe.MU_Len_Min, "Мин", AppColors.ChartLengthMin_Line, AppColors.ChartLengthMin_Label, "F0")
            {
                FontSize = AppFonts.ChartLengthMin_FontSize,
                FontWeight = AppFonts.ChartLengthMin_FontWeight,
                FontFamily = AppFonts.ChartLengthMin_FontFamily
            };
            var lengthMaxLine = new ConstantLineViewModel(recipe.MU_Len_Max, "Макс", AppColors.ChartLengthMax_Line, AppColors.ChartLengthMax_Label, "F0")
            {
                FontSize = AppFonts.ChartLengthMax_FontSize,
                FontWeight = AppFonts.ChartLengthMax_FontWeight,
                FontFamily = AppFonts.ChartLengthMax_FontFamily
            };
            var lengthDump = new ConstantLineViewModel(recipe.MU_Len_Dump, "сброс", AppColors.ChartLengthDump_Line, AppColors.ChartLengthDump_Label, "F0")
            {
                FontSize = AppFonts.ChartLengthDump_FontSize,
                FontWeight = AppFonts.ChartLengthDump_FontWeight,
                FontFamily = AppFonts.ChartLengthDump_FontFamily
            };
            var shoulderMinLine = new ConstantLineViewModel(recipe.MU_TqShoulder_Min, "Мин. буртик", AppColors.ChartShoulderMin_Line, AppColors.ChartShoulderMin_Label, "F0")
            {
                FontSize = AppFonts.ChartShoulderMin_FontSize,
                FontWeight = AppFonts.ChartShoulderMin_FontWeight,
                FontFamily = AppFonts.ChartShoulderMin_FontFamily
            };
            var shoulderMaxLine = new ConstantLineViewModel(recipe.MU_TqShoulder_Max, "Макс. буртик", AppColors.ChartShoulderMax_Line, AppColors.ChartShoulderMax_Label, "F0")
            {
                FontSize = AppFonts.ChartShoulderMax_FontSize,
                FontWeight = AppFonts.ChartShoulderMax_FontWeight,
                FontFamily = AppFonts.ChartShoulderMax_FontFamily
            };

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
        /// <summary>
        /// Нарисовать выделенные области по данным рецепта
        /// </summary>
        /// <param name="recipe"></param>
        private void UpdateStrips(JointRecipe recipe)
        {
            // Создаем выделенные области для допустимых диапазонов
            var torqueStrip = new StripViewModel(
                recipe.MU_Tq_Min,
                recipe.MU_Tq_Max,
                AppColors.ChartStripTorque);

            var lengthStrip = new StripViewModel(
                recipe.MU_Len_Min,
                recipe.MU_Len_Max,
                AppColors.ChartStripLength);

            var shoulderStrip = new StripViewModel(
                recipe.MU_TqShoulder_Min,
                recipe.MU_TqShoulder_Max,
                AppColors.ChartStripShoulder);

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
        #endregion

        #region Настройка графиков при начале свинчивания

        /// <summary>
        /// Настроить графики при появлении трубы
        /// </summary>
        /// <param name="result"></param>
        public void UpdatePipeApper(JointResult result)
        {
            TorqueLengthChart.XMin = result.MVS_Len_mm;
        }

        #endregion

        #region Обработка изменений коллекции точек

        /// <summary>
        /// Обработчик изменений коллекции TqTnLenPoints
        /// </summary>
        private void OnTqTnLenPointsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Reset:
                    // Обновляем данные для всех графиков
                    TorqueTurnsChart.ChartData = TqTnLenPoints;
                    TurnsPerMinuteTurnsChart.ChartData = TqTnLenPoints;
                    TorqueLengthChart.ChartData = TqTnLenPoints;
                    TorqueTimeChart.ChartData = TqTnLenPoints;
                    break;
            }
        }

        #endregion
    }
}
