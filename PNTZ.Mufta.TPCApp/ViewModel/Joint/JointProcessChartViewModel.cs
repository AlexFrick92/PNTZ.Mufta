using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.ViewModel.Control;

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
