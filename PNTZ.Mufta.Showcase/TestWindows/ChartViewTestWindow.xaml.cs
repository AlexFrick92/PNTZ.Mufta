using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using PNTZ.Mufta.TPCApp.ViewModel.Control;
using PNTZ.Mufta.Showcase.Data;

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
            _viewModel.XMax = 50;
            _viewModel.YMax = 10000;

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
            _viewModel.XMax = 100;
            _viewModel.YMax = 10000;

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
            _viewModel.XMax = 50;
            _viewModel.YMax = 10000;

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
            _viewModel.XMax = 50;
            _viewModel.YMax = 10000;

            UpdateStatus($"Загружены случайные данные ({pointCount} точек)");
        }

        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            StopRealtime();
            _viewModel.ChartData = null;
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
            UpdateStatus($"Real-time симуляция запущена (интервал: {interval} мс)");
        }

        private void PauseRealtime_Click(object sender, RoutedEventArgs e)
        {
            if (_realtimeTimer.IsEnabled)
            {
                _realtimeTimer.Stop();
                UpdateStatus($"Real-time симуляция приостановлена (точек: {_currentIndex}/{_fullDataSet?.Count ?? 0})");
            }
            else if (_fullDataSet != null && _currentIndex < _fullDataSet.Count)
            {
                _realtimeTimer.Start();
                UpdateStatus("Real-time симуляция возобновлена");
            }
        }

        private void StopRealtime_Click(object sender, RoutedEventArgs e)
        {
            StopRealtime();
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

        #region Вспомогательные методы

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
