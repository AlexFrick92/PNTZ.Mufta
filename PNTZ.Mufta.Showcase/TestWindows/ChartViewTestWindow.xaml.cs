using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using PNTZ.Mufta.TPCApp.ViewModel.Control;
using PNTZ.Mufta.Showcase.Data;
using System.Linq;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования контрола ChartView
    /// </summary>
    public partial class ChartViewTestWindow : Window
    {
        private ChartViewModel _viewModel;
        private DispatcherTimer _realtimeTimer;
        private List<TorquePoint> _realtimeDataPoints;
        private List<TorquePoint> _fullDataSet;
        private int _currentIndex = 0;
        private Random _random = new Random();

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
        }

        /// <summary>
        /// Инициализация ViewModel для графика
        /// </summary>
        private void InitializeViewModel()
        {
            _viewModel = new ChartViewModel
            {
                ChartTitle = "Тестирование графика крутящего момента",
                ArgumentMember = "Turns",
                ValueMember = "Torque",
                XMin = 0,
                XMax = 50,
                YMin = 0,
                YMax = 10000,
                LineColor = new SolidColorBrush(Color.FromRgb(52, 152, 219)), // Синий
                LineThickness = 2.5
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

        #region Обработчики кнопок - Тестовые данные

        private void LoadRealisticData_Click(object sender, RoutedEventArgs e)
        {
            StopRealtime();

            int pointCount = GetPointCount();
            var data = MockDataGenerator.GenerateRealisticTorqueData(
                pointCount: pointCount,
                maxTurns: 50,
                maxTorque: 8000);

            _viewModel.ChartData = data;
            SetAxisValues(0, 50, 0, 10000);

            // Обновляем цвета кнопок
            ResetDataButtonColors();
            ResetRealtimeButtonColors();
            BtnRealisticData.Background = _activeGreenColor;
            BtnStop.Background = _activeRedColor;

            UpdateStatus($"Загружены реалистичные данные ({pointCount} точек)");
        }

        private void LoadSineWave_Click(object sender, RoutedEventArgs e)
        {
            StopRealtime();

            int pointCount = GetPointCount();
            var data = MockDataGenerator.GenerateSineWaveData(
                pointCount: pointCount,
                amplitude: 4000,
                frequency: 3);

            _viewModel.ChartData = data;
            SetAxisValues(0, 100, 0, 10000);

            // Обновляем цвета кнопок
            ResetDataButtonColors();
            ResetRealtimeButtonColors();
            BtnSineWave.Background = _activeBlueColor;
            BtnStop.Background = _activeRedColor;

            UpdateStatus($"Загружена синусоида ({pointCount} точек)");
        }

        private void LoadLinearData_Click(object sender, RoutedEventArgs e)
        {
            StopRealtime();

            int pointCount = GetPointCount();
            var data = MockDataGenerator.GenerateLinearData(
                pointCount: pointCount,
                maxTurns: 50,
                maxTorque: 10000);

            _viewModel.ChartData = data;
            SetAxisValues(0, 50, 0, 10000);

            // Обновляем цвета кнопок
            ResetDataButtonColors();
            ResetRealtimeButtonColors();
            BtnLinearData.Background = _activePurpleColor;
            BtnStop.Background = _activeRedColor;

            UpdateStatus($"Загружены линейные данные ({pointCount} точек)");
        }

        private void LoadRandomData_Click(object sender, RoutedEventArgs e)
        {
            StopRealtime();

            int pointCount = GetPointCount();
            var data = MockDataGenerator.GenerateRandomData(
                pointCount: pointCount,
                maxTurns: 50,
                maxTorque: 10000);

            _viewModel.ChartData = data;
            SetAxisValues(0, 50, 0, 10000);

            // Обновляем цвета кнопок
            ResetDataButtonColors();
            ResetRealtimeButtonColors();
            BtnRandomData.Background = _activeOrangeColor;
            BtnStop.Background = _activeRedColor;

            UpdateStatus($"Загружены случайные данные ({pointCount} точек)");
        }

        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            StopRealtime();
            _viewModel.ChartData = null;

            // Обновляем цвета кнопок
            ResetDataButtonColors();
            ResetRealtimeButtonColors();
            BtnClearData.Background = _activeRedColor;
            BtnStop.Background = _activeRedColor;

            UpdateStatus("График очищен");
        }

        #endregion

        #region Обработчики кнопок - Real-time

        private void StartRealtime_Click(object sender, RoutedEventArgs e)
        {
            // Генерируем полный набор данных
            _fullDataSet = MockDataGenerator.GenerateRealisticTorqueData(
                pointCount: GetPointCount(),
                maxTurns: 50,
                maxTorque: 8000);

            // Подготавливаем коллекцию для отображения
            _realtimeDataPoints = new List<TorquePoint>();
            _currentIndex = 0;

            // Настраиваем интервал таймера
            int interval = GetInterval();
            _realtimeTimer.Interval = TimeSpan.FromMilliseconds(interval);

            // Запускаем таймер
            _realtimeTimer.Start();

            // Обновляем цвета кнопок
            ResetDataButtonColors();
            ResetRealtimeButtonColors();
            BtnStart.Background = _activeGreenColor;

            UpdateStatus($"Real-time симуляция запущена (интервал: {interval} мс)");
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
            if (_fullDataSet == null || _currentIndex >= _fullDataSet.Count)
            {
                _realtimeTimer.Stop();

                // Обновляем цвета кнопок - симуляция завершена
                ResetRealtimeButtonColors();

                UpdateStatus($"Real-time симуляция завершена ({_fullDataSet?.Count ?? 0} точек)");
                return;
            }

            // Добавляем следующую точку
            _realtimeDataPoints.Add(_fullDataSet[_currentIndex]);
            _currentIndex++;

            // Обновляем график (пересоздаем коллекцию для обновления UI)
            var updatedData = new List<TorquePoint>(_realtimeDataPoints);
            _viewModel.ChartData = updatedData;

            // Обновляем статус
            UpdateStatus($"Real-time: {_currentIndex}/{_fullDataSet.Count} точек");
        }

        #endregion

        #region Обработчики кнопок - Константные линии

        private void AddXLine_Click(object sender, RoutedEventArgs e)
        {
            double value = _random.Next(10, 40);

            _viewModel.XConstantLines.Add(new ConstantLineViewModel
            {
                Value = value,
                Label = $"X-{_viewModel.XConstantLines.Count + 1}",
                Color = GetRandomBrush()
            });

            UpdateStatus($"Добавлена константная линия X = {value:F1}");
        }

        private void AddYLine_Click(object sender, RoutedEventArgs e)
        {
            double value = _random.Next(3000, 8000);

            _viewModel.YConstantLines.Add(new ConstantLineViewModel
            {
                Value = value,
                Label = $"Y-{_viewModel.YConstantLines.Count + 1}",
                Color = GetRandomBrush()
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
                var data = _viewModel.ChartData as System.Collections.Generic.IEnumerable<TorquePoint>;
                if (data != null)
                {
                    var points = new List<TorquePoint>(data);
                    if (points.Count > 0)
                    {
                        var minX = points.Min(p => p.Turns);
                        var maxX = points.Max(p => p.Turns);
                        var minY = points.Min(p => p.Torque);
                        var maxY = points.Max(p => p.Torque);

                        // Добавляем небольшой отступ (5%)
                        var xMargin = (maxX - minX) * 0.05;
                        var yMargin = (maxY - minY) * 0.05;

                        SetAxisValues(
                            minX - xMargin,
                            maxX + xMargin,
                            minY - yMargin,
                            maxY + yMargin
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
        /// Получает количество точек из TextBox
        /// </summary>
        private int GetPointCount()
        {
            if (int.TryParse(PointCountTextBox.Text, out int count) && count > 0 && count <= 10000)
            {
                return count;
            }
            return 100; // Значение по умолчанию
        }

        /// <summary>
        /// Получает интервал обновления из TextBox
        /// </summary>
        private int GetInterval()
        {
            if (int.TryParse(IntervalTextBox.Text, out int interval) && interval > 0 && interval <= 5000)
            {
                return interval;
            }
            return 50; // Значение по умолчанию
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
