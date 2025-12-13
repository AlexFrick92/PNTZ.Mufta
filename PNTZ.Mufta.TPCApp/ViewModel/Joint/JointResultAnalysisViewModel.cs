using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Styles;
using PNTZ.Mufta.TPCApp.ViewModel.Control;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace PNTZ.Mufta.TPCApp.ViewModel.Joint
{
    /// <summary>
    /// ViewModel для JointResultAnalysisView.
    /// Отображает график анализа результата свинчивания с дополнительными вычисленными параметрами
    /// </summary>
    public class JointResultAnalysisViewModel : BaseViewModel
    {
        private JointResult _currentResult;
        private ObservableCollection<AnalysisDataPoint> _analysisData;

        /// <summary>
        /// График: Момент/обороты с двумя линиями (оригинальный и сглаженный)
        /// </summary>
        public ChartViewModel AnalysisChart { get; private set; }

        /// <summary>
        /// Текущий результат для анализа
        /// </summary>
        public JointResult CurrentResult
        {
            get => _currentResult;
            set
            {
                _currentResult = value;
                OnPropertyChanged(nameof(CurrentResult));
                UpdateAnalysisData();
            }
        }

        public JointResultAnalysisViewModel()
        {
            _analysisData = new ObservableCollection<AnalysisDataPoint>();
            InitializeChart();
        }

        /// <summary>
        /// Инициализация графика
        /// </summary>
        private void InitializeChart()
        {
            AnalysisChart = new ChartViewModel
            {
                ArgumentMember = "Turns",
                XAxisTitle = "Обороты",
                YAxisTitle = "Момент",
                XMin = 0,
                XMax = 10,
                YMin = 0,
                YMax = 100
            };

            // Создаем две серии:
            // 1. Оригинальный момент
            var torqueSeries = new ChartSeriesViewModel(
                "Torque",
                "Момент",
                AppColors.ChartTorqueTurns_Line as SolidColorBrush ?? Brushes.Blue,
                2.0);

            // 2. Сглаженный момент
            var smoothedTorqueSeries = new ChartSeriesViewModel(
                "SmoothedTorque",
                "Момент (сглаженный)",
                AppColors.ChartTorqueLength_Line as SolidColorBrush ?? Brushes.Red,
                2.0);

            AnalysisChart.Series.Add(torqueSeries);
            AnalysisChart.Series.Add(smoothedTorqueSeries);

            AnalysisChart.ChartData = _analysisData;

            OnPropertyChanged(nameof(AnalysisChart));
        }

        /// <summary>
        /// Обновление данных анализа на основе текущего результата
        /// </summary>
        private void UpdateAnalysisData()
        {
            _analysisData.Clear();

            if (_currentResult?.Series == null || _currentResult.Series.Count == 0)
                return;

            // Преобразуем TqTnLenPoint в AnalysisDataPoint
            foreach (var point in _currentResult.Series)
            {
                var analysisPoint = AnalysisDataPoint.FromTqTnLenPoint(point);
                _analysisData.Add(analysisPoint);
            }

            // Автоматически подгоняем границы графика под данные
            FitChartToData();
        }

        /// <summary>
        /// Подгонка границ графика под данные
        /// </summary>
        private void FitChartToData()
        {
            if (_analysisData == null || _analysisData.Count == 0)
                return;

            const double margin = 0.1; // 10% отступ

            // Находим минимальные и максимальные значения
            var minTurns = _analysisData.Min(p => p.Turns);
            var maxTurns = _analysisData.Max(p => p.Turns);
            var minTorque = _analysisData.Min(p => p.Torque);
            var maxTorque = _analysisData.Max(p => p.Torque);

            // Вычисляем диапазоны
            var turnsRange = maxTurns - minTurns;
            var torqueRange = maxTorque - minTorque;

            // Устанавливаем границы с отступом
            AnalysisChart.XMin = minTurns;
            AnalysisChart.XMax = maxTurns + turnsRange * margin;
            AnalysisChart.YMin = minTorque;
            AnalysisChart.YMax = maxTorque + torqueRange * margin;
        }

        /// <summary>
        /// Очистка графика
        /// </summary>
        public void ClearChart()
        {
            _analysisData.Clear();
            _currentResult = null;
            OnPropertyChanged(nameof(CurrentResult));
        }
    }
}
