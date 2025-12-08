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
        private ChartViewModel _torqueTurnsChart;
        private ChartViewModel _turnsPerMinuteTurnsChart;
        private ChartViewModel _torqueLengthChart;
        private ChartViewModel _torqueTimeChart;

        /// <summary>
        /// График: Момент/обороты
        /// </summary>
        public ChartViewModel TorqueTurnsChart
        {
            get => _torqueTurnsChart;
            set
            {
                _torqueTurnsChart = value;
                OnPropertyChanged(nameof(TorqueTurnsChart));
            }
        }

        /// <summary>
        /// График: (Обороты/Мин)/обороты
        /// </summary>
        public ChartViewModel TurnsPerMinuteTurnsChart
        {
            get => _turnsPerMinuteTurnsChart;
            set
            {
                _turnsPerMinuteTurnsChart = value;
                OnPropertyChanged(nameof(TurnsPerMinuteTurnsChart));
            }
        }

        /// <summary>
        /// График: Момент/длина
        /// </summary>
        public ChartViewModel TorqueLengthChart
        {
            get => _torqueLengthChart;
            set
            {
                _torqueLengthChart = value;
                OnPropertyChanged(nameof(TorqueLengthChart));
            }
        }

        /// <summary>
        /// График: Момент/время
        /// </summary>
        public ChartViewModel TorqueTimeChart
        {
            get => _torqueTimeChart;
            set
            {
                _torqueTimeChart = value;
                OnPropertyChanged(nameof(TorqueTimeChart));
            }
        }

        public JointProcessChartViewModel()
        {
            InitializeCharts();
        }

        private void InitializeCharts()
        {
            // График: Момент/обороты
            TorqueTurnsChart = new ChartViewModel
            {
                ChartTitle = "Момент/обороты",
                ArgumentMember = "Turns",
                XAxisTitle = "Обороты",
                YAxisTitle = "Момент",
                XMin = 0,
                XMax = 0,
                YMin = 0,
                YMax = 30000
            };

            // График: (Обороты/Мин)/обороты
            TurnsPerMinuteTurnsChart = new ChartViewModel
            {
                ChartTitle = "(Обороты/Мин)/обороты",
                ArgumentMember = "Turns",
                XAxisTitle = "Обороты",
                YAxisTitle = "Обороты/Мин",
                XMin = 0,
                XMax = 5,
                YMin = 0,
                YMax = 60
            };

            // График: Момент/длина
            TorqueLengthChart = new ChartViewModel
            {
                ChartTitle = "Момент/длина",
                ArgumentMember = "Length",
                XAxisTitle = "Длина",
                YAxisTitle = "Момент",
                XMin = 0,
                XMax = 0,
                YMin = 0,
                YMax = 30000
            };

            // График: Момент/время
            TorqueTimeChart = new ChartViewModel
            {
                ChartTitle = "Момент/Время",
                ArgumentMember = "TimeStamp",
                XAxisTitle = "Время",
                YAxisTitle = "Момент",
                XMin = 0,
                XMax = 90000,
                YMin = 0,
                YMax = 30000
            };
        }
    }
}
