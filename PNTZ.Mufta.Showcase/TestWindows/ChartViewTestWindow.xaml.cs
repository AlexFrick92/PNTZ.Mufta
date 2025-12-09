using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using PNTZ.Mufta.TPCApp.ViewModel.Control;
using PNTZ.Mufta.Showcase.Data;
using System.Linq;
using System.Windows.Controls;
using PNTZ.Mufta.TPCApp.View.Formatter;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования контрола ChartView
    /// </summary>
    public partial class ChartViewTestWindow : Window
    {
        private ChartViewModel _viewModel;
        private DispatcherTimer _realtimeTimer;
        private List<SeriesPoint> _realtimeDataPoints;
        private List<SeriesPoint> _fullDataSet;
        private int _currentIndex = 0;
        private Random _random = new Random();
        private int _currentInterval = 50; // Текущий интервал в миллисекундах

        // Мастер-коллекция с общей сеткой X для всех серий
        private List<SeriesPoint> _masterData;

        // Трекинг серий: какая кнопка использует какое поле YValN
        private class SeriesTracker
        {
            public string YValField { get; set; }  // "YVal1", "YVal2", etc.
            public bool IsActive { get; set; }      // Активна ли серия
            public SolidColorBrush Color { get; set; }
            public string DisplayName { get; set; }
        }

        private Dictionary<Button, SeriesTracker> _seriesTracking;

        // Флаг для предотвращения рекурсивного обновления осей
        private bool _isUpdatingAxes = false;

        // Цвета кнопок
        private readonly SolidColorBrush _inactiveColor = new SolidColorBrush(Color.FromRgb(149, 165, 166)); // #95A5A6 - серый
        private readonly SolidColorBrush _activeGreenColor = new SolidColorBrush(Color.FromRgb(39, 174, 96)); // #27AE60 - зеленый
        private readonly SolidColorBrush _activeOrangeColor = new SolidColorBrush(Color.FromRgb(243, 156, 18)); // #F39C12 - оранжевый
        private readonly SolidColorBrush _activeRedColor = new SolidColorBrush(Color.FromRgb(231, 76, 60)); // #E74C3C - красный
        private readonly SolidColorBrush _activeBlueColor = new SolidColorBrush(Color.FromRgb(52, 152, 219)); // #3498DB - синий
        private readonly SolidColorBrush _activePurpleColor = new SolidColorBrush(Color.FromRgb(155, 89, 182)); // #9B59B6 - фиолетовый

        public ChartViewTestWindow()
        {
            InitializeComponent();
            InitializeViewModel();
            InitializeRealtimeTimer();
            InitializeSeriesTracking();
        }

        /// <summary>
        /// Инициализация ViewModel для графика
        /// </summary>
        private void InitializeViewModel()
        {
            _viewModel = new ChartViewModel
            {
                ChartTitle = "Тестирование графика крутящего момента",
                ArgumentMember = "XVal",
                XMin = 0,
                XMax = 5000,
                YMin = 0,
                YMax = 10000,
                XAxisTitle = "Cек",
                YAxisTitle = "Нм",
                YAxisLabelFormatter = new ThousandsDoubleLabelFormatter(),
                XAxisLabelFormatter = new SecondsIntegerLabelFormatter(),

            };

            ChartView.DataContext = _viewModel;
            UpdateStatus("График инициализирован. Готов к тестированию.");
        }

        /// <summary>
        /// Инициализация таймера для real-time обновлений
        /// </summary>
        private void InitializeRealtimeTimer()
        {
            _realtimeTimer = new DispatcherTimer();
            _realtimeTimer.Tick += RealtimeTimer_Tick;
        }

        /// <summary>
        /// Инициализация трекинга серий для кнопок
        /// </summary>
        private void InitializeSeriesTracking()
        {
            _seriesTracking = new Dictionary<Button, SeriesTracker>
            {
                { BtnRealisticData, new SeriesTracker { YValField = "YVal1", IsActive = false, Color = _activeGreenColor, DisplayName = "Реалистичные" } },
                { BtnSineWave, new SeriesTracker { YValField = "YVal2", IsActive = false, Color = _activeBlueColor, DisplayName = "Синусоида" } },
                { BtnLinearData, new SeriesTracker { YValField = "YVal3", IsActive = false, Color = _activePurpleColor, DisplayName = "Линейный рост" } },
                { BtnRandomData, new SeriesTracker { YValField = "YVal4", IsActive = false, Color = _activeOrangeColor, DisplayName = "Случайные" } }
            };
        }

        #region Обработчики кнопок - Тестовые данные

        private void LoadRealisticData_Click(object sender, RoutedEventArgs e)
        {
            StopRealtime();
            double yMax = GetAxisValue(YMaxTextBox, 10000);
            ToggleSeries(BtnRealisticData, xValues => MockDataGenerator.GenerateRealisticTorqueData(xValues, yMax));
        }

        private void LoadSineWave_Click(object sender, RoutedEventArgs e)
        {
            StopRealtime();
            double yMax = GetAxisValue(YMaxTextBox, 10000);
            ToggleSeries(BtnSineWave, xValues => MockDataGenerator.GenerateSineWaveData(xValues, yMax / 2.5, 3));
        }

        private void LoadLinearData_Click(object sender, RoutedEventArgs e)
        {
            StopRealtime();
            double yMax = GetAxisValue(YMaxTextBox, 10000);
            ToggleSeries(BtnLinearData, xValues => MockDataGenerator.GenerateLinearData(xValues, yMax));
        }

        private void LoadRandomData_Click(object sender, RoutedEventArgs e)
        {
            StopRealtime();
            double yMax = GetAxisValue(YMaxTextBox, 10000);
            ToggleSeries(BtnRandomData, xValues => MockDataGenerator.GenerateRandomData(xValues, yMax));
        }

        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            StopRealtime();

            // Очищаем мастер-коллекцию
            _masterData = null;

            // Очищаем все серии
            _viewModel.Series.Clear();
            _viewModel.ChartData = null;

            // Очищаем константные линии и Strip'ы
            _viewModel.XConstantLines.Clear();
            _viewModel.YConstantLines.Clear();
            _viewModel.XStrips.Clear();
            _viewModel.YStrips.Clear();

            // Сбрасываем состояние трекеров
            foreach (var tracker in _seriesTracking.Values)
            {
                tracker.IsActive = false;
            }

            // Сбрасываем цвета кнопок
            ResetDataButtonColors();
            ResetRealtimeButtonColors();

            UpdateStatus("График очищен. Коллекция данных, линии и области сброшены.");
        }

        #endregion

        #region Обработчики кнопок - Real-time

        private void StartRealtime_Click(object sender, RoutedEventArgs e)
        {
            // Инициализируем мастер-коллекцию, если нужно
            if (_masterData == null || _masterData.Count == 0)
            {
                InitializeMasterData();
            }

            // Получаем параметры
            double yMax = _viewModel.YMax;
            int interval = GetInterval();

            // Используем YVal1 для real-time (можно изменить на любой свободный)
            string realtimeField = "YVal1";

            // Генерируем полный набор данных Y
            int[] xValues = _masterData.Select(p => p.XVal).ToArray();
            _fullDataSet = new List<SeriesPoint>(); // Переиспользуем для хранения только Y-значений

            double[] yValues = MockDataGenerator.GenerateRealisticTorqueData(xValues, yMax);
            for (int i = 0; i < yValues.Length; i++)
            {
                _fullDataSet.Add(new SeriesPoint { XVal = xValues[i], YVal1 = yValues[i] });
            }

            // Сброс индекса
            _currentIndex = 0;

            // Обнуляем YVal1 перед началом
            foreach (var point in _masterData)
            {
                point.YVal1 = 0;
            }

            // Добавляем серию для real-time, если её нет
            if (!_viewModel.Series.Any(s => s.DisplayName == "Real-time"))
            {
                _viewModel.Series.Add(new ChartSeriesViewModel(
                    realtimeField,
                    "Real-time",
                    _activeGreenColor,
                    2.0
                ));
            }

            // Настраиваем таймер
            _realtimeTimer.Interval = TimeSpan.FromMilliseconds(interval);
            _realtimeTimer.Start();

            // Обновляем цвета кнопок
            ResetRealtimeButtonColors();
            BtnStart.Background = _activeGreenColor;

            double totalTime = (_masterData.Count * interval) / 1000.0;
            UpdateStatus($"Real-time симуляция запущена: {_masterData.Count} точек, ~{totalTime:F1} сек");
        }

        private void PauseRealtime_Click(object sender, RoutedEventArgs e)
        {
            if (_realtimeTimer.IsEnabled)
            {
                _realtimeTimer.Stop();

                // Обновляем цвета кнопок
                ResetRealtimeButtonColors();
                BtnPause.Background = _activeOrangeColor;

                UpdateStatus($"Real-time симуляция приостановлена (точек: {_currentIndex}/{_fullDataSet?.Count ?? 0})");
            }
            else if (_fullDataSet != null && _currentIndex < _fullDataSet.Count)
            {
                _realtimeTimer.Start();

                // Обновляем цвета кнопок
                ResetRealtimeButtonColors();
                BtnStart.Background = _activeGreenColor;

                UpdateStatus("Real-time симуляция возобновлена");
            }
        }

        private void StopRealtime_Click(object sender, RoutedEventArgs e)
        {
            StopRealtime();

            // Обновляем цвета кнопок
            ResetRealtimeButtonColors();
            BtnStop.Background = _activeRedColor;
        }

        private void StopRealtime()
        {
            _realtimeTimer.Stop();
            _fullDataSet = null;
            _realtimeDataPoints = null;
            _currentIndex = 0;
            UpdateStatus("Real-time симуляция остановлена");
        }

        private void RealtimeTimer_Tick(object sender, EventArgs e)
        {
            if (_fullDataSet == null || _currentIndex >= _fullDataSet.Count || _masterData == null)
            {
                _realtimeTimer.Stop();

                // Обновляем цвета кнопок - симуляция завершена
                ResetRealtimeButtonColors();

                UpdateStatus($"Real-time симуляция завершена ({_fullDataSet?.Count ?? 0} точек)");
                return;
            }

            // Обновляем следующую точку в мастер-коллекции
            _masterData[_currentIndex].YVal1 = _fullDataSet[_currentIndex].YVal1;
            _currentIndex++;

            // Обновляем график (пересоздаем коллекцию для обновления UI)
            _viewModel.ChartData = new List<SeriesPoint>(_masterData);

            // Обновляем статус
            UpdateStatus($"Real-time: {_currentIndex}/{_fullDataSet.Count} точек");
        }

        #endregion

        #region Обработчики кнопок - Константные линии

        private void AddXLine_Click(object sender, RoutedEventArgs e)
        {
            // Генерируем значение в диапазоне 20-80% от XMax
            double xRange = _viewModel.XMax - _viewModel.XMin;
            double value = _viewModel.XMin + xRange * (0.2 + _random.NextDouble() * 0.6);

            var color = GetRandomBrush();
            _viewModel.XConstantLines.Add(new ConstantLineViewModel
            {
                Value = value,
                Label = $"X-{_viewModel.XConstantLines.Count + 1}",
                LineColor = color,
                LabelColor = color,
                ValueFormat = "F0"
            });

            UpdateStatus($"Добавлена константная линия X = {value:F1}");
        }

        private void AddYLine_Click(object sender, RoutedEventArgs e)
        {
            // Генерируем значение в диапазоне 20-80% от YMax
            double yRange = _viewModel.YMax - _viewModel.YMin;
            double value = _viewModel.YMin + yRange * (0.2 + _random.NextDouble() * 0.6);

            var color = GetRandomBrush();
            _viewModel.YConstantLines.Add(new ConstantLineViewModel
            {
                Value = value,
                Label = $"Y-{_viewModel.YConstantLines.Count + 1}",
                LineColor = color,
                LabelColor = color,
                ValueFormat = "F1"
            });

            UpdateStatus($"Добавлена константная линия Y = {value:F0}");
        }

        private void ClearLines_Click(object sender, RoutedEventArgs e)
        {
            int totalLines = _viewModel.XConstantLines.Count + _viewModel.YConstantLines.Count;
            _viewModel.XConstantLines.Clear();
            _viewModel.YConstantLines.Clear();
            UpdateStatus($"Очищено {totalLines} константных линий");
        }

        #endregion

        #region Обработчики кнопок - Выделенные области (Strips)

        private void AddXStrip_Click(object sender, RoutedEventArgs e)
        {
            // Генерируем диапазон в пределах 20-80% от XMax
            double xRange = _viewModel.XMax - _viewModel.XMin;
            double minValue = _viewModel.XMin + xRange * (0.2 + _random.NextDouble() * 0.4);
            double maxValue = minValue + xRange * (0.1 + _random.NextDouble() * 0.2);

            _viewModel.XStrips.Add(new StripViewModel
            {
                MinValue = minValue,
                MaxValue = maxValue,
                Color = GetRandomTransparentBrush()
            });

            UpdateStatus($"Добавлена выделенная область X: [{minValue:F1}, {maxValue:F1}]");
        }

        private void AddYStrip_Click(object sender, RoutedEventArgs e)
        {
            // Генерируем диапазон в пределах 20-80% от YMax
            double yRange = _viewModel.YMax - _viewModel.YMin;
            double minValue = _viewModel.YMin + yRange * (0.2 + _random.NextDouble() * 0.4);
            double maxValue = minValue + yRange * (0.1 + _random.NextDouble() * 0.2);

            _viewModel.YStrips.Add(new StripViewModel
            {
                MinValue = minValue,
                MaxValue = maxValue,
                Color = GetRandomTransparentBrush()
            });

            UpdateStatus($"Добавлена выделенная область Y: [{minValue:F0}, {maxValue:F0}]");
        }

        private void ClearStrips_Click(object sender, RoutedEventArgs e)
        {
            int totalStrips = _viewModel.XStrips.Count + _viewModel.YStrips.Count;
            _viewModel.XStrips.Clear();
            _viewModel.YStrips.Clear();
            UpdateStatus($"Очищено {totalStrips} выделенных областей");
        }

        #endregion

        #region Обработчики кнопок - Интервал

        private void SetInterval10_Click(object sender, RoutedEventArgs e)
        {
            SetInterval(10);
        }

        private void SetInterval50_Click(object sender, RoutedEventArgs e)
        {
            SetInterval(50);
        }

        private void SetInterval100_Click(object sender, RoutedEventArgs e)
        {
            SetInterval(100);
        }

        /// <summary>
        /// Устанавливает интервал и обновляет подсветку кнопок
        /// </summary>
        private void SetInterval(int interval)
        {
            _currentInterval = interval;

            // Сбрасываем цвета всех кнопок интервала
            BtnInterval10.Background = _inactiveColor;
            BtnInterval50.Background = _inactiveColor;
            BtnInterval100.Background = _inactiveColor;

            // Подсвечиваем выбранную кнопку
            switch (interval)
            {
                case 10:
                    BtnInterval10.Background = _activeGreenColor;
                    break;
                case 50:
                    BtnInterval50.Background = _activeGreenColor;
                    break;
                case 100:
                    BtnInterval100.Background = _activeGreenColor;
                    break;
            }

            UpdateStatus($"Интервал изменён на {interval} мс");
        }

        #endregion

        #region Обработчики параметров осей

        /// <summary>
        /// Обработчик изменения параметров осей
        /// </summary>
        private void AxisParameter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_isUpdatingAxes || _viewModel == null)
                return;

            UpdateAxesFromTextBoxes();
        }

        /// <summary>
        /// Обработчик кнопки "Сброс зума"
        /// </summary>
        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            // Устанавливаем значения по умолчанию в зависимости от текущих данных
            if (_viewModel.ChartData != null)
            {
                // Если есть данные, подгоняем оси под данные
                var data = _viewModel.ChartData as System.Collections.Generic.IEnumerable<SeriesPoint>;
                if (data != null)
                {
                    var points = new List<SeriesPoint>(data);
                    if (points.Count > 0)
                    {
                        var minX = points.Min(p => p.XVal);
                        var maxX = points.Max(p => p.XVal);
                        var minY = points.Min(p => p.YVal1);
                        var maxY = points.Max(p => p.YVal1);

                        // Добавляем небольшой отступ (5%)
                        var xMargin = (maxX - minX) * 0.05;
                        var yMargin = (maxY - minY) * 0.05;

                        SetAxisValues(
                            minX, //- xMargin,
                            maxX, //+ xMargin,
                            minY, //- yMargin,
                            maxY //+ yMargin
                        );

                        UpdateStatus("Зум сброшен - оси подогнаны под данные");
                        return;
                    }
                }
            }

            // Если данных нет, используем значения по умолчанию
            SetAxisValues(0, 50, 0, 10000);
            UpdateStatus("Зум сброшен - установлены значения по умолчанию");
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Инициализирует мастер-коллекцию с общей сеткой X для всех серий
        /// </summary>
        private void InitializeMasterData()
        {
            double xMin = _viewModel.XMin;
            double xMax = _viewModel.XMax;
            int interval = GetInterval();

            // Рассчитываем количество точек
            double xRange = xMax - xMin;
            int pointCount = (int)Math.Ceiling(xRange / interval);

            // Создаём коллекцию с заполненными значениями X
            _masterData = new List<SeriesPoint>(pointCount);
            for (int i = 0; i < pointCount; i++)
            {
                _masterData.Add(new SeriesPoint
                {
                    XVal = (int)(xMin + i * interval),
                    YVal1 = 0,
                    YVal2 = 0,
                    YVal3 = 0,
                    YVal4 = 0
                });
            }

            UpdateStatus($"Инициализирована коллекция: {pointCount} точек, интервал {interval} мс, диапазон {xMin}-{xMax} мс");
        }

        /// <summary>
        /// Универсальный обработчик toggle для кнопок добавления данных
        /// </summary>
        private void ToggleSeries(Button button, Func<int[], double[]> dataGenerator)
        {
            if (!_seriesTracking.ContainsKey(button))
                return;

            var tracker = _seriesTracking[button];

            // Проверяем, активна ли серия
            if (tracker.IsActive)
            {
                // Удаляем серию
                var seriesToRemove = _viewModel.Series.FirstOrDefault(s => s.ValueMember == tracker.YValField);
                if (seriesToRemove != null)
                {
                    _viewModel.Series.Remove(seriesToRemove);
                }

                // Обнуляем данные в мастер-коллекции
                if (_masterData != null)
                {
                    foreach (var point in _masterData)
                    {
                        switch (tracker.YValField)
                        {
                            case "YVal1": point.YVal1 = 0; break;
                            case "YVal2": point.YVal2 = 0; break;
                            case "YVal3": point.YVal3 = 0; break;
                            case "YVal4": point.YVal4 = 0; break;
                        }
                    }
                    _viewModel.ChartData = new List<SeriesPoint>(_masterData);
                }

                tracker.IsActive = false;
                button.Background = _inactiveColor;
                UpdateStatus($"Серия '{tracker.DisplayName}' удалена");
            }
            else
            {
                // Инициализируем мастер-коллекцию, если нужно
                if (_masterData == null || _masterData.Count == 0)
                {
                    InitializeMasterData();
                }

                // Получаем массив X
                int[] xValues = _masterData.Select(p => p.XVal).ToArray();

                // Генерируем данные Y
                double[] yValues = dataGenerator(xValues);

                // Заполняем YValN в мастер-коллекции
                for (int i = 0; i < _masterData.Count && i < yValues.Length; i++)
                {
                    switch (tracker.YValField)
                    {
                        case "YVal1": _masterData[i].YVal1 = yValues[i]; break;
                        case "YVal2": _masterData[i].YVal2 = yValues[i]; break;
                        case "YVal3": _masterData[i].YVal3 = yValues[i]; break;
                        case "YVal4": _masterData[i].YVal4 = yValues[i]; break;
                    }
                }

                // Добавляем серию
                _viewModel.Series.Add(new ChartSeriesViewModel(
                    tracker.YValField,
                    tracker.DisplayName,
                    tracker.Color,
                    2.0
                ));

                // Обновляем график
                _viewModel.ChartData = new List<SeriesPoint>(_masterData);

                tracker.IsActive = true;
                button.Background = tracker.Color;
                UpdateStatus($"Серия '{tracker.DisplayName}' добавлена ({_masterData.Count} точек)");
            }
        }

        /// <summary>
        /// Сбрасывает цвета всех кнопок выбора данных на неактивное состояние
        /// </summary>
        private void ResetDataButtonColors()
        {
            BtnRealisticData.Background = _inactiveColor;
            BtnSineWave.Background = _inactiveColor;
            BtnLinearData.Background = _inactiveColor;
            BtnRandomData.Background = _inactiveColor;
            BtnClearData.Background = _inactiveColor;
        }

        /// <summary>
        /// Сбрасывает цвета всех кнопок real-time управления на неактивное состояние
        /// </summary>
        private void ResetRealtimeButtonColors()
        {
            BtnStart.Background = _inactiveColor;
            BtnPause.Background = _inactiveColor;
            BtnStop.Background = _inactiveColor;
        }

        /// <summary>
        /// Обновляет оси графика из TextBox'ов
        /// </summary>
        private void UpdateAxesFromTextBoxes()
        {
            if (_isUpdatingAxes)
                return;

            try
            {
                double xMin = double.TryParse(XMinTextBox.Text, out double xMinVal) ? xMinVal : 0;
                double xMax = double.TryParse(XMaxTextBox.Text, out double xMaxVal) ? xMaxVal : 50;
                double yMin = double.TryParse(YMinTextBox.Text, out double yMinVal) ? yMinVal : 0;
                double yMax = double.TryParse(YMaxTextBox.Text, out double yMaxVal) ? yMaxVal : 10000;

                _viewModel.XMin = xMin;
                _viewModel.XMax = xMax;
                _viewModel.YMin = yMin;
                _viewModel.YMax = yMax;
            }
            catch
            {
                // Игнорируем ошибки парсинга
            }
        }

        /// <summary>
        /// Устанавливает значения осей и обновляет TextBox'ы
        /// </summary>
        private void SetAxisValues(double xMin, double xMax, double yMin, double yMax)
        {
            _isUpdatingAxes = true;

            try
            {
                // Обновляем ViewModel
                _viewModel.XMin = xMin;
                _viewModel.XMax = xMax;
                _viewModel.YMin = yMin;
                _viewModel.YMax = yMax;

                // Обновляем TextBox'ы
                XMinTextBox.Text = xMin.ToString("F2");
                XMaxTextBox.Text = xMax.ToString("F2");
                YMinTextBox.Text = yMin.ToString("F2");
                YMaxTextBox.Text = yMax.ToString("F2");
            }
            finally
            {
                _isUpdatingAxes = false;
            }
        }

        /// <summary>
        /// Получает интервал обновления (миллисекунды)
        /// </summary>
        private int GetInterval()
        {
            return _currentInterval;
        }

        /// <summary>
        /// Получает значение оси из TextBox с fallback на значение по умолчанию
        /// </summary>
        private double GetAxisValue(System.Windows.Controls.TextBox textBox, double defaultValue)
        {
            if (textBox != null && double.TryParse(textBox.Text, out double value) && value > 0)
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Обновляет статусную строку
        /// </summary>
        private void UpdateStatus(string message)
        {
            StatusText.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        }

        /// <summary>
        /// Генерирует случайную кисть для константных линий
        /// </summary>
        private SolidColorBrush GetRandomBrush()
        {
            var colors = new[]
            {
                Color.FromRgb(231, 76, 60),   // Красный
                Color.FromRgb(230, 126, 34),  // Оранжевый
                Color.FromRgb(241, 196, 15),  // Желтый
                Color.FromRgb(46, 204, 113),  // Зеленый
                Color.FromRgb(52, 152, 219),  // Синий
                Color.FromRgb(155, 89, 182),  // Фиолетовый
                Color.FromRgb(52, 73, 94)     // Темно-синий
            };

            return new SolidColorBrush(colors[_random.Next(colors.Length)]);
        }

        /// <summary>
        /// Генерирует случайную полупрозрачную кисть для выделенных областей
        /// </summary>
        private SolidColorBrush GetRandomTransparentBrush()
        {
            var colors = new[]
            {
                Color.FromRgb(231, 76, 60),   // Красный
                Color.FromRgb(230, 126, 34),  // Оранжевый
                Color.FromRgb(241, 196, 15),  // Желтый
                Color.FromRgb(46, 204, 113),  // Зеленый
                Color.FromRgb(52, 152, 219),  // Синий
                Color.FromRgb(155, 89, 182),  // Фиолетовый
                Color.FromRgb(52, 73, 94)     // Темно-синий
            };

            return new SolidColorBrush(colors[_random.Next(colors.Length)]) { Opacity = 0.2 };
        }

        #endregion

        /// <summary>
        /// Очистка ресурсов при закрытии окна
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            _realtimeTimer?.Stop();
            base.OnClosed(e);
        }
    }
}
