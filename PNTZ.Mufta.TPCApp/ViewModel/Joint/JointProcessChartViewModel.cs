using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Styles;
using PNTZ.Mufta.TPCApp.ViewModel.Control;
using Promatis.Core.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

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
            //_tqTnLenPoints.CollectionChanged += OnTqTnLenPointsChanged;
            InitializeCharts();
            InitializeUpdateTimer();
        }
        private ObservableCollection<TqTnLenPoint> _tqTnLenPoints = new ObservableCollection<TqTnLenPoint>();
        private DispatcherTimer _pointUpdateTimer = new DispatcherTimer(DispatcherPriority.Background);
        private int _pointCounter = 0; // Счетчик для throttling обновления границ графиков
        private ConstantLineViewModel _shoulderPointYLine;
        private ConstantLineViewModel _shoulderPointXLine;
        private JointRecipe _actualRecipe;

        #region публичные свойства и методы
        /// <summary>
        /// Очередь добавления точек из фонового потока
        /// </summary>
        public ConcurrentQueue<TqTnLenPoint> TqTnLenPointsQueue { get; private set; } = new ConcurrentQueue<TqTnLenPoint>();        
        /// <summary>
        /// Очистить графики
        /// </summary>
        public void ClearCharts()
        {
            if (Application.Current.Dispatcher.CheckAccess())
                _tqTnLenPoints.Clear();
            else
                Application.Current.Dispatcher.Invoke(() => _tqTnLenPoints.Clear());

            while (TqTnLenPointsQueue.TryDequeue(out var point)) { }
            _pointCounter = 0;

            if (_shoulderPointXLine != null)
            {
                TorqueTurnsChart.XConstantLines.Remove(_shoulderPointXLine);
                _shoulderPointXLine = null; 
            }
            if (_shoulderPointYLine != null)
            {
                TorqueTurnsChart.YConstantLines.Remove(_shoulderPointYLine);
                _shoulderPointYLine = null;
            }
        }
        /// <summary>
        /// Настроить графики при появлении трубы
        /// </summary>
        /// <param name="result"></param>
        public void PipeAppear(JointResult result)
        {
            TorqueLengthChart.XMin = result.MVS_Len_mm;
            ClearCharts();
        }
        /// <summary>
        /// Запуск записи графиков
        /// </summary>
        public void RecordingBegin()
        {                        
            _pointUpdateTimer.Start();            
        }
        public void RecordingStop()
        {
            _pointUpdateTimer.Stop();
            UpdateChartBoundsIfNeeded(_tqTnLenPoints.Last());
        }
        public void AutoEvaluationResult(JointResult result)
        {
            if(_actualRecipe.JointMode == JointMode.TorqueShoulder)
            {
                _shoulderPointXLine = new ConstantLineViewModel(result.FinalShoulderTurns, "Буртик", AppColors.ChartShoulderMin_Line, AppColors.ChartShoulderMin_Label, "F0")
                {
                    FontSize = AppFonts.ChartShoulderMin_FontSize,
                    FontWeight = AppFonts.ChartShoulderMin_FontWeight,
                    FontFamily = AppFonts.ChartShoulderMin_FontFamily
                };
                _shoulderPointYLine = new ConstantLineViewModel(result.FinalShoulderTorque, "Буртик", AppColors.ChartShoulderMax_Line, AppColors.ChartShoulderMax_Label, "F0")
                {
                    FontSize = AppFonts.ChartShoulderMax_FontSize,
                    FontWeight = AppFonts.ChartShoulderMax_FontWeight,
                    FontFamily = AppFonts.ChartShoulderMax_FontFamily
                };
                
                TorqueTurnsChart.XConstantLines.Add(_shoulderPointXLine);
                TorqueTurnsChart.YConstantLines.Add(_shoulderPointYLine);
            }
        }

        /// <summary>
        /// Свинчивание завершено
        /// </summary>
        /// <param name="result"></param>
        public void FinishJointing(JointResult result)
        {
            if (result.ResultTotal == 1)
                FitChartsToData();
            _pointUpdateTimer.Stop();
        }
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
                ArgumentMember = "Length_mm",
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

            TorqueTurnsChart.ChartData = _tqTnLenPoints;
            TurnsPerMinuteTurnsChart.ChartData = _tqTnLenPoints;
            TorqueLengthChart.ChartData = _tqTnLenPoints;
            TorqueTimeChart.ChartData = _tqTnLenPoints;
            // Уведомляем View о готовности всех графиков
            OnPropertyChanged(nameof(TorqueTurnsChart));
            OnPropertyChanged(nameof(TurnsPerMinuteTurnsChart));
            OnPropertyChanged(nameof(TorqueLengthChart));
            OnPropertyChanged(nameof(TorqueTimeChart));
        }
        #endregion

        private void InitializeUpdateTimer()
        {
            _pointUpdateTimer.Interval = TimeSpan.FromMilliseconds(AppSettings.ChartUpdateInterval);
            _pointUpdateTimer.Tick += (s, e) =>
            {
                TqTnLenPoint latestPoint = null;
                int pointsAdded = 0;
                while (TqTnLenPointsQueue.TryDequeue(out var point))
                {
                    latestPoint = point;
                    _tqTnLenPoints.Add(point);
                    pointsAdded++;
                }

                if (latestPoint != null)
                {
                    _pointCounter += pointsAdded;
                    // Обновляем границы только через каждые N точек (из настроек)
                    if (_pointCounter >= AppSettings.ChartBoundsUpdateFrequency)
                    {
                        UpdateChartBoundsIfNeeded(latestPoint);
                        _pointCounter = 0;
                    }
                }
            };
        }
        // Проверяет, не выходит ли новая точка за границы графиков,
        // и расширяет границы при необходимости с запасом
        private void UpdateChartBoundsIfNeeded(TqTnLenPoint point)
        {
            // График: Момент/обороты
            ExpandBoundsIfNeeded(TorqueTurnsChart, point.Turns, point.Torque, AppSettings.ChartMargin);
            // График: (Обороты/Мин)/обороты
            ExpandBoundsIfNeeded(TurnsPerMinuteTurnsChart, point.Turns, point.TurnsPerMinute, AppSettings.ChartMargin);
            // График: Момент/длина
            ExpandBoundsIfNeeded(TorqueLengthChart, point.Length_mm, point.Torque, AppSettings.ChartMargin);
            // График: Момент/время
            ExpandBoundsIfNeeded(TorqueTimeChart, point.TimeStamp, point.Torque, AppSettings.ChartMargin);
        }

        // Расширяет границы графика если значения выходят за пределы
        private void ExpandBoundsIfNeeded(ChartViewModel chart, double xValue, double yValue, double margin)
        {
            // Проверка и расширение оси X
            if (xValue < chart.XMin)
            {
                double delta = chart.XMin - xValue;
                chart.XMin = xValue - delta * margin;
            }
            else if (xValue > chart.XMax)
            {
                double delta = xValue - chart.XMax;
                chart.XMax = xValue + delta * margin;
            }

            // Проверка и расширение оси Y
            if (yValue < chart.YMin)
            {
                double delta = chart.YMin - yValue;
                chart.YMin = yValue - delta * margin;
            }
            else if (yValue > chart.YMax)
            {
                double delta = yValue - chart.YMax;
                chart.YMax = yValue + delta * margin;
            }
        }

        #region Настройка графиков по данным рецепта       
        /// <summary>
        /// Настроить графики под рецепт
        /// </summary>
        /// <param name="recipe"></param>
        public void UpdateRecipe(JointRecipe recipe)
        {
            _actualRecipe = recipe;
            UpdateRanges(recipe);
            UpdateConstantLines(recipe);
            UpdateStrips(recipe);
        }
        /// Задать диапазон графиков по данным рцепта
        private void UpdateRanges(JointRecipe recipe)
        {
            //График: Момент/Обороты
            TorqueTurnsChart.YMax = recipe.MU_Tq_Max * (1 + AppSettings.ChartMargin);
            TorqueTurnsChart.YMin = 0;
            TorqueTurnsChart.XMax = 3;
            TorqueTurnsChart.XMin = 0;

            //График: Момент/Длина
            TorqueLengthChart.YMax = recipe.MU_Tq_Max * (1 + AppSettings.ChartMargin);
            TorqueLengthChart.YMin = 0;
            TorqueLengthChart.XMax = recipe.MU_Len_Max * (1 + AppSettings.ChartMargin);
            TorqueLengthChart.XMin = 0;

            //График: Обороты/ОборотыВминуту
            TurnsPerMinuteTurnsChart.YMax = 100;
            TurnsPerMinuteTurnsChart.YMin = 0;
            TurnsPerMinuteTurnsChart.XMax = 3;
            TurnsPerMinuteTurnsChart.XMin = 0;

            //График: Момент/Время
            TorqueTimeChart.YMax = recipe.MU_Tq_Max * (1 + AppSettings.ChartMargin);
            TorqueTimeChart.YMin = 0;
            TorqueTimeChart.XMax = 15000;
            TorqueTimeChart.XMin = 0;
        }
        /// Нарисовать прямые линии по данным рецепта
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
        /// Нарисовать выделенные области по данным рецепта
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

        #region Настройка графиков по данным точки
        // Подгоняет границы всех графиков под финальные данные с небольшим отступом
        private void FitChartsToData()
        {
            if (_tqTnLenPoints == null || _tqTnLenPoints.Count == 0)
                return;

            // График: Момент/обороты
            FitChartBounds(
                TorqueTurnsChart,
                _tqTnLenPoints.Min(p => p.Turns),
                _tqTnLenPoints.Max(p => p.Turns),
                _tqTnLenPoints.Min(p => p.Torque),
                _tqTnLenPoints.Max(p => p.Torque),
                AppSettings.ChartMargin,
                adjustXMin: true,
                adjustYMin: true);

            // График: (Обороты/Мин)/обороты
            FitChartBounds(
                TurnsPerMinuteTurnsChart,
                _tqTnLenPoints.Min(p => p.Turns),
                _tqTnLenPoints.Max(p => p.Turns),
                _tqTnLenPoints.Min(p => p.TurnsPerMinute),
                _tqTnLenPoints.Max(p => p.TurnsPerMinute),
                AppSettings.ChartMargin,
                adjustXMin: true,
                adjustYMin: true);

            // График: Момент/длина (XMin сохраняем текущее значение)
            FitChartBounds(
                TorqueLengthChart,
                _tqTnLenPoints.Min(p => p.Length_mm),
                _tqTnLenPoints.Max(p => p.Length_mm),
                _tqTnLenPoints.Min(p => p.Torque),
                _tqTnLenPoints.Max(p => p.Torque),
                AppSettings.ChartMargin,
                adjustXMin: false, // Сохраняем XMin установленный в PipeAppear
                adjustYMin: true);

            // График: Момент/время
            FitChartBounds(
                TorqueTimeChart,
                _tqTnLenPoints.Min(p => p.TimeStamp),
                _tqTnLenPoints.Max(p => p.TimeStamp),
                _tqTnLenPoints.Min(p => p.Torque),
                _tqTnLenPoints.Max(p => p.Torque),
                AppSettings.ChartMargin,
                adjustXMin: true,
                adjustYMin: true);
        }
        /// Устанавливает границы графика с учетом данных, ConstantLines и Strips
        private void FitChartBounds(
            ChartViewModel chart,
            double dataXMin,
            double dataXMax,
            double dataYMin,
            double dataYMax,
            double margin,
            bool adjustXMin,
            bool adjustYMin)
        {
            // Учитываем ConstantLines по оси X
            if (chart.XConstantLines.Any())
            {
                var xLineMin = chart.XConstantLines.Min(line => line.Value);
                var xLineMax = chart.XConstantLines.Max(line => line.Value);
                dataXMin = Math.Min(dataXMin, xLineMin);
                dataXMax = Math.Max(dataXMax, xLineMax);
            }

            // Учитываем Strips по оси X
            if (chart.XStrips.Any())
            {
                var xStripMin = chart.XStrips.Min(strip => strip.MinValue);
                var xStripMax = chart.XStrips.Max(strip => strip.MaxValue);
                dataXMin = Math.Min(dataXMin, xStripMin);
                dataXMax = Math.Max(dataXMax, xStripMax);
            }

            // Учитываем ConstantLines по оси Y
            if (chart.YConstantLines.Any())
            {
                var yLineMin = chart.YConstantLines.Min(line => line.Value);
                var yLineMax = chart.YConstantLines.Max(line => line.Value);
                dataYMin = Math.Min(dataYMin, yLineMin);
                dataYMax = Math.Max(dataYMax, yLineMax);
            }

            // Учитываем Strips по оси Y
            if (chart.YStrips.Any())
            {
                var yStripMin = chart.YStrips.Min(strip => strip.MinValue);
                var yStripMax = chart.YStrips.Max(strip => strip.MaxValue);
                dataYMin = Math.Min(dataYMin, yStripMin);
                dataYMax = Math.Max(dataYMax, yStripMax);
            }

            // Вычисляем диапазоны с отступом
            double xRange = dataXMax - dataXMin;
            double yRange = dataYMax - dataYMin;

            // Устанавливаем границы оси X
            if (adjustXMin)
            {
                chart.XMin = dataXMin;
            }
            chart.XMax = dataXMax + xRange * margin;

            // Устанавливаем границы оси Y
            if (adjustYMin)
            {
                chart.YMin = dataYMin;
            }
            chart.YMax = dataYMax + yRange * margin;
        }
        #endregion
    }
}
