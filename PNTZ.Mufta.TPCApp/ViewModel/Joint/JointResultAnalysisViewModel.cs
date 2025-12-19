using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.Styles;
using PNTZ.Mufta.TPCApp.ViewModel.Control;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace PNTZ.Mufta.TPCApp.ViewModel.Joint
{
    /// <summary>
    /// ViewModel для JointResultAnalysisView.
    /// Отображает график анализа результата свинчивания с дополнительными вычисленными параметрами
    /// </summary>
    public class JointResultAnalysisViewModel : BaseViewModel
    {
        private JointResultTable _currentResult;
        private ObservableCollection<AnalysisDataPoint> _analysisData;
        private string _detectionResultText;

        /// <summary>
        /// График: Момент/обороты с двумя линиями (оригинальный и сглаженный)
        /// </summary>
        public ChartViewModel AnalysisChart { get; private set; }

        /// <summary>
        /// Текущий результат для анализа
        /// </summary>
        public JointResultTable CurrentResult
        {
            get => _currentResult;
            set
            {
                _currentResult = value;
                OnPropertyChanged(nameof(CurrentResult));

                // Очистить результаты предыдущего расчёта
                ClearDetectionVisualization();
                _lastDetectionResult = null;
                DetectionResultText = "Расчёт не выполнен";

                UpdateAnalysisData();
                UpdateSearchAreaVisualization();
            }
        }

        public JointResultAnalysisViewModel()
        {
            _analysisData = new ObservableCollection<AnalysisDataPoint>();
            _detectionResultText = "Расчёт не выполнен";
            InitializeChart();
            Calculate = new RelayCommand(RunShoulderDetection);
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

            // 2. Сглаженный момент (из AnalysisDataPoint.SmoothedTorque, не от детектора)
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

            if (_currentResult?.PointSeries == null || _currentResult.PointSeries.Count == 0)
                return;

            // Преобразуем TqTnLenPoint в AnalysisDataPoint
            foreach (var point in _currentResult.PointSeries)
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
            AnalysisChart.YMin = minTorque - 2000;
            AnalysisChart.YMax = maxTorque + torqueRange * margin;
        }

        #region Shoulder Detection

        private int _windowSize = 30;
        private int _derivativeWindowSize = 15;
        private double _sigmaMultiplier = 10.0;
        private double _searchStartRatio = 0.7;
        private ShoulderDetectionResult _lastDetectionResult;

        // Свойства видимости элементов визуализации
        private bool _showDerivative = true;
        private bool _showSmoothedTorque = true;
        private bool _showSigmaLines = true;
        private bool _showThreshold = true;
        private bool _showBaseline = true;

        /// <summary>
        /// Размер окна для сглаживания момента
        /// </summary>
        public int WindowSize
        {
            get => _windowSize;
            set
            {
                _windowSize = value;
                Debug.WriteLine($"WindowSize set {value}");
                OnPropertyChanged(nameof(WindowSize));
            }
        }

        /// <summary>
        /// Размер окна для сглаживания производной
        /// </summary>
        public int DerivativeWindowSize
        {
            get => _derivativeWindowSize;
            set
            {
                _derivativeWindowSize = value;
                OnPropertyChanged(nameof(DerivativeWindowSize));
            }
        }

        /// <summary>
        /// Множитель сигма для определения порога
        /// </summary>
        public double SigmaMultiplier
        {
            get => _sigmaMultiplier;
            set
            {
                _sigmaMultiplier = value;
                OnPropertyChanged(nameof(SigmaMultiplier));
            }
        }

        /// <summary>
        /// С какого места начинать искать заплечник (0.0 - 1.0)
        /// </summary>
        public double SearchStartRatio
        {
            get => _searchStartRatio;
            set
            {
                _searchStartRatio = value;
                OnPropertyChanged(nameof(SearchStartRatio));
                UpdateSearchAreaVisualization();
            }
        }

        /// <summary>
        /// С какого места начинать искать заплечник (в процентах, 0 - 100)
        /// </summary>
        public double SearchStartPercent
        {
            get => _searchStartRatio * 100.0;
            set
            {
                _searchStartRatio = value / 100.0;
                OnPropertyChanged(nameof(SearchStartPercent));
                OnPropertyChanged(nameof(SearchStartRatio));
                UpdateSearchAreaVisualization();
            }
        }

        /// <summary>
        /// Отображать производную на графике
        /// </summary>
        public bool ShowDerivative
        {
            get => _showDerivative;
            set
            {
                _showDerivative = value;
                OnPropertyChanged(nameof(ShowDerivative));
                UpdateVisualizationVisibility();
            }
        }

        /// <summary>
        /// Отображать сглаженный момент на графике
        /// </summary>
        public bool ShowSmoothedTorque
        {
            get => _showSmoothedTorque;
            set
            {
                _showSmoothedTorque = value;
                OnPropertyChanged(nameof(ShowSmoothedTorque));
                UpdateVisualizationVisibility();
            }
        }

        /// <summary>
        /// Отображать линии сигм на графике
        /// </summary>
        public bool ShowSigmaLines
        {
            get => _showSigmaLines;
            set
            {
                _showSigmaLines = value;
                OnPropertyChanged(nameof(ShowSigmaLines));
                UpdateVisualizationVisibility();
            }
        }

        /// <summary>
        /// Отображать линию порога (threshold) на графике
        /// </summary>
        public bool ShowThreshold
        {
            get => _showThreshold;
            set
            {
                _showThreshold = value;
                OnPropertyChanged(nameof(ShowThreshold));
                UpdateVisualizationVisibility();
            }
        }

        /// <summary>
        /// Отображать базовую линию (baseline) на графике
        /// </summary>
        public bool ShowBaseline
        {
            get => _showBaseline;
            set
            {
                _showBaseline = value;
                OnPropertyChanged(nameof(ShowBaseline));
                UpdateVisualizationVisibility();
            }
        }

        /// <summary>
        /// Текст результата детектора заплечника
        /// </summary>
        public string DetectionResultText
        {
            get => _detectionResultText;
            set
            {
                _detectionResultText = value;
                OnPropertyChanged(nameof(DetectionResultText));
            }
        }

        /// <summary>
        /// Запуск детектора точки контакта с заплечником
        /// </summary>
        public void RunShoulderDetection(object arg)
        {
            try
            {
                if (_currentResult?.PointSeries == null || _currentResult.PointSeries.Count == 0)
                {
                    DetectionResultText = "⚠ Результат не загружен";
                    return;
                }

                DetectionResultText = "⏳ Выполняется расчёт...";

                // Очистить предыдущую визуализацию
                ClearDetectionVisualization();

                Debug.WriteLine($"WindowSize : {WindowSize}");

                // Создать детектор с текущими параметрами
                var detector = new ShoulderPointDetector(_currentResult.PointSeries)
                {
                    WindowSize = WindowSize,
                    DerivativeWindowSize = DerivativeWindowSize,
                    SigmaMultiplier = SigmaMultiplier,
                    SearchStartRatio = SearchStartRatio
                };

                // Запустить анализ
                _lastDetectionResult = detector.DetectShoulderPoint();

                // Визуализировать результаты
                VisualizeDetectionResults(_lastDetectionResult);

                // Сформировать текст результата
                if (_lastDetectionResult?.ShoulderPointIndex.HasValue == true)
                {
                    int index = _lastDetectionResult.ShoulderPointIndex.Value;
                    var point = _currentResult.PointSeries[index];

                    DetectionResultText = $"✓ Точка найдена!\n" +
                        $"Индекс: {index}\n" +
                        $"Момент: {point.Torque:F1} Nm\n" +
                        $"Обороты: {point.Turns:F3}\n" +
                        $"Время: {point.TimeStamp / 1000.0:F2} сек";
                }
                else
                {
                    DetectionResultText = "✗ Точка заплечника не найдена";
                }
            }
            catch (Exception ex)
            {
                DetectionResultText = $"⚠ Ошибка: {ex.Message}";
            }
        }

        /// <summary>
        /// Получить результат последнего расчёта детектора
        /// </summary>
        public ShoulderDetectionResult GetLastDetectionResult()
        {
            return _lastDetectionResult;
        }

        /// <summary>
        /// Визуализация результатов детектора на графике
        /// </summary>
        private void VisualizeDetectionResults(ShoulderDetectionResult result)
        {
            if (result == null)
                return;

            // 1. Добавить серию сглаженного момента
            if (ShowSmoothedTorque)
            {
                AddSmoothedTorqueSeries(result);
            }

            // 2. Добавить серию нормализованной производной
            if (ShowDerivative)
            {
                AddNormalizedDerivativeSeries(result);
            }

            // 3. Добавить Strip для области поиска заплечника
            AddSearchAreaStrip(result);

            // 4. Добавить константные линии для baseline и порогов
            AddBaselineAndThresholdLines(result);

            // 5. Добавить маркер найденной точки заплечника
            if (result.ShoulderPointIndex.HasValue)
            {
                AddShoulderPointMarker(result.ShoulderPointIndex.Value);
            }
        }

        /// <summary>
        /// Добавление серии сглаженного момента на график
        /// </summary>
        private void AddSmoothedTorqueSeries(ShoulderDetectionResult result)
        {
            if (result.SmoothedTorque == null || result.SmoothedTorque.Count == 0)
                return;

            // Заполнить сглаженный момент в данных
            for (int i = 0; i < result.SmoothedTorque.Count && i < _analysisData.Count; i++)
            {
                _analysisData[i].SmoothedTorqueFromDetector = (float)result.SmoothedTorque[i];
            }

            // Добавить серию на график
            var smoothedTorqueSeries = new ChartSeriesViewModel(
                "SmoothedTorqueFromDetector",
                "Момент (сглаженный)",
                Brushes.Green, // Зелёный для сглаженного момента
                2.5);

            AnalysisChart.Series.Add(smoothedTorqueSeries);
        }

        /// <summary>
        /// Добавление серии нормализованной производной на график
        /// </summary>
        private void AddNormalizedDerivativeSeries(ShoulderDetectionResult result)
        {
            if (result.SmoothedDerivatives == null || result.WindowCenters == null)
                return;

            // Найти диапазон момента для нормализации производной
            var minTorque = _analysisData.Min(p => p.Torque);
            var maxTorque = _analysisData.Max(p => p.Torque);

            // Нормализовать производную к диапазону момента
            for (int i = 0; i < result.SmoothedDerivatives.Count; i++)
            {
                int pointIndex = result.WindowCenters[i];
                if (pointIndex < _analysisData.Count)
                {
                    double normalizedDerivative = NormalizeValue(
                        result.SmoothedDerivatives[i],
                        result.DerivativeMin,
                        result.DerivativeMax,
                        minTorque,
                        maxTorque);

                    _analysisData[pointIndex].TorqueDerivative = (float)normalizedDerivative;
                }
            }

            // Добавить серию на график
            var derivativeSeries = new ChartSeriesViewModel(
                "TorqueDerivative",
                "Производная (норм.)",
                Brushes.DarkViolet, // Фиолетовый - хорошо контрастирует с синим и красным
                2.0);

            AnalysisChart.Series.Add(derivativeSeries);
        }

        /// <summary>
        /// Добавление Strip для выделения области поиска заплечника
        /// </summary>
        private void AddSearchAreaStrip(ShoulderDetectionResult result)
        {
            // Strip уже отображается через UpdateSearchAreaVisualization()
            // Ничего не делаем, чтобы не дублировать
        }

        /// <summary>
        /// Добавление константных линий для baseline и порогов
        /// </summary>
        private void AddBaselineAndThresholdLines(ShoulderDetectionResult result)
        {
            // Нормализовать значения baseline и threshold к диапазону момента
            var minTorque = _analysisData.Min(p => p.Torque);
            var maxTorque = _analysisData.Max(p => p.Torque);

            double normalizedBaseline = NormalizeValue(
                result.BaselineAverage,
                result.DerivativeMin,
                result.DerivativeMax,
                minTorque,
                maxTorque);

            // Baseline (только если ShowBaseline = true)
            if (ShowBaseline)
            {
                AnalysisChart.YConstantLines.Add(new ConstantLineViewModel
                {
                    Value = normalizedBaseline,
                    Label = "Baseline",
                    Color = Brushes.DarkGreen, // Тёмно-зелёный для базовой линии
                    Thickness = 2.0,
                    LineStyle = LineStyle.Dash
                });
            }

            // Линии для каждой сигмы до порога
            int sigmaCount = (int)Math.Ceiling(SigmaMultiplier);
            for (int i = 1; i <= sigmaCount; i++)
            {
                double sigmaValue = result.BaselineAverage + i * result.BaselineStdDev;
                double normalizedSigma = NormalizeValue(
                    sigmaValue,
                    result.DerivativeMin,
                    result.DerivativeMax,
                    minTorque,
                    maxTorque);

                bool isThreshold = i == sigmaCount;
                var color = isThreshold ? Brushes.Blue : Brushes.DarkGray; // Голубой для порога, серый для σ
                var label = isThreshold ? $"Threshold (+{SigmaMultiplier:F1}σ)" : $"+{i}σ";

                // Проверяем флаги видимости
                bool shouldShow = isThreshold ? ShowThreshold : ShowSigmaLines;

                if (shouldShow)
                {
                    AnalysisChart.YConstantLines.Add(new ConstantLineViewModel
                    {
                        Value = normalizedSigma,
                        Label = label,
                        Color = color,
                        Thickness = isThreshold ? 1.5 : 1.0,
                        LineStyle = isThreshold ? LineStyle.Solid : LineStyle.Dot
                    });
                }
            }
        }

        /// <summary>
        /// Добавление маркера найденной точки заплечника
        /// </summary>
        private void AddShoulderPointMarker(int shoulderIndex)
        {
            if (shoulderIndex >= _analysisData.Count)
                return;

            double shoulderTurns = _analysisData[shoulderIndex].Turns;

            AnalysisChart.XConstantLines.Add(new ConstantLineViewModel
            {
                Value = shoulderTurns,
                Label = "Точка заплечника",
                Color = Brushes.OrangeRed, // Оранжево-красный - отличается от Crimson порога
                Thickness = 2.0,
                LineStyle = LineStyle.Solid
            });
        }

        /// <summary>
        /// Очистка визуализации результатов детектора
        /// </summary>
        private void ClearDetectionVisualization()
        {
            // Удалить серию производной
            var derivativeSeries = AnalysisChart.Series.FirstOrDefault(s => s.ValueMember == "TorqueDerivative");
            if (derivativeSeries != null)
            {
                AnalysisChart.Series.Remove(derivativeSeries);
            }

            // Удалить серию сглаженного момента
            var smoothedTorqueSeries = AnalysisChart.Series.FirstOrDefault(s => s.ValueMember == "SmoothedTorqueFromDetector");
            if (smoothedTorqueSeries != null)
            {
                AnalysisChart.Series.Remove(smoothedTorqueSeries);
            }

            // Очистить производную и сглаженный момент в данных
            foreach (var point in _analysisData)
            {
                point.TorqueDerivative = 0f;
                point.SmoothedTorqueFromDetector = 0f;
            }

            // Очистить константные линии (но НЕ XStrips - зона поиска управляется отдельно)
            AnalysisChart.XConstantLines.Clear();
            AnalysisChart.YConstantLines.Clear();
            AnalysisChart.YStrips.Clear();
        }

        /// <summary>
        /// Обновление видимости элементов визуализации при изменении чекбоксов
        /// </summary>
        private void UpdateVisualizationVisibility()
        {
            if (_lastDetectionResult == null)
                return;

            // Повторно визуализировать с учетом новых настроек
            ClearDetectionVisualization();
            VisualizeDetectionResults(_lastDetectionResult);
        }

        /// <summary>
        /// Нормализация значения из одного диапазона в другой
        /// </summary>
        private double NormalizeValue(double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            if (Math.Abs(fromMax - fromMin) < 0.0001)
                return toMin;

            double normalized = (value - fromMin) / (fromMax - fromMin);
            return toMin + normalized * (toMax - toMin);
        }

        /// <summary>
        /// Обновление визуализации зоны поиска заплечника
        /// Вызывается при изменении SearchStartRatio или загрузке нового результата
        /// </summary>
        private void UpdateSearchAreaVisualization()
        {
            // Очистить существующие strips
            AnalysisChart.XStrips.Clear();

            if (_analysisData == null || _analysisData.Count == 0)
                return;

            // Вычислить точку начала поиска
            int totalPoints = _analysisData.Count;
            int searchStartIndex = (int)(totalPoints * SearchStartRatio);

            if (searchStartIndex >= totalPoints)
                return;

            double searchStartTurns = _analysisData[searchStartIndex].Turns;
            double maxTurns = _analysisData.Max(p => p.Turns);

            // Добавить strip для зоны поиска
            var searchAreaStrip = new StripViewModel
            {
                MinValue = searchStartTurns,
                MaxValue = maxTurns,
                Color = new SolidColorBrush(Color.FromArgb(50, 135, 206, 250)) // Полупрозрачный голубой (SkyBlue)
            };

            AnalysisChart.XStrips.Add(searchAreaStrip);
        }

        #endregion

        /// <summary>
        /// Очистка графика
        /// </summary>
        public void ClearChart()
        {
            _analysisData.Clear();
            _currentResult = null;
            OnPropertyChanged(nameof(CurrentResult));
        }
        public ICommand Calculate { get; private set; }
    }
}
