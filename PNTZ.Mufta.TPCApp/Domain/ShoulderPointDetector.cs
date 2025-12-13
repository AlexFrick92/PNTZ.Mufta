using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PNTZ.Mufta.TPCApp.Toolbox.Smoothing;

namespace PNTZ.Mufta.TPCApp.Domain
{
    /// <summary>
    /// Детектор точки контакта муфты с заплечником (буртиком) трубы.
    /// Анализирует график зависимости момента от длины/оборотов для определения точки резкого возрастания момента.
    /// </summary>
    public class ShoulderPointDetector
    {
        private List<TqTnLenPoint> _series;

        /// <summary>
        /// Размер окна для сглаживания момента (борьба с шумом)
        /// </summary>
        public int WindowSize { get; set; } = 30;

        /// <summary>
        /// Размер окна для сглаживания производной
        /// </summary>
        public int DerivativeWindowSize { get; set; } = 15;

        /// <summary>
        /// Множитель сигма для определения порога (чувствительность детектора)
        /// Меньше = раньше срабатывает, больше = позже срабатывает
        /// </summary>
        public double SigmaMultiplier { get; set; } = 10;

        /// <summary>
        /// С какого места начинать искать заплечник (доля от общей длины)
        /// </summary>
        public double SearchStartRatio { get; set; } = 0.7;

        /// <summary>
        /// Шаг для вычисления производной
        /// </summary>
        public int Step { get; set; } = 1;

        public ShoulderPointDetector(List<TqTnLenPoint> series)
        {
            _series = series ?? throw new ArgumentNullException(nameof(series));
        }

        public ShoulderDetectionResult DetectShoulderPoint()
        {
            var result = new ShoulderDetectionResult();

            Console.WriteLine("=== Universal Shoulder Detection ===");
            Console.WriteLine($"Total points: {_series.Count}");
            Console.WriteLine($"Parameters:");
            Console.WriteLine($"  WindowSize (torque smoothing): {WindowSize}");
            Console.WriteLine($"  DerivativeWindowSize (derivative smoothing): {DerivativeWindowSize}");
            Console.WriteLine($"  SigmaMultiplier (sensitivity): {SigmaMultiplier}");
            Console.WriteLine($"  SearchStartRatio: {SearchStartRatio}");
            Console.WriteLine();

            if (_series == null || _series.Count < WindowSize * 2)
            {
                Console.WriteLine("ERROR: Not enough points for analysis");
                return result;
            }

            // Фаза 1: Найти максимум момента и отсечь фазу разгрузки
            int maxIndex = FindMaxTorqueIndex();
            int analyzeEnd = maxIndex;
            result.MaxTorqueIndex = maxIndex;

            float maxTorque = _series[maxIndex].Torque;
            Console.WriteLine($"Phase 4 (Unloading): Max torque {maxTorque:F1} Nm at index {maxIndex}");
            Console.WriteLine("Analyzing up to maximum, excluding unloading phase");
            Console.WriteLine();

            // Фаза 1.5: Вычислить сглаженный момент (ПЕРЕД расчётом производной!)
            Console.WriteLine($"Smoothing torque signal (window={WindowSize} points)...");
            var smoothedTorque = CalculateSmoothedTorque();
            result.SmoothedTorque = smoothedTorque;

            // Фаза 2: Вычислить производную от СГЛАЖЕННОГО момента
            Console.WriteLine($"Calculating derivatives from smoothed torque...");
            var (avgDerivatives, windowCenters) = CalculateMovingAverageDerivatives(analyzeEnd, smoothedTorque);

            if (avgDerivatives.Count == 0)
            {
                Console.WriteLine("ERROR: No derivatives calculated");
                return result;
            }

            result.SmoothedDerivatives = avgDerivatives;
            result.WindowCenters = windowCenters;

            // Вычислить min/max производной для нормализации
            result.DerivativeMin = avgDerivatives.Min();
            result.DerivativeMax = avgDerivatives.Max();

            Console.WriteLine($"Calculated {avgDerivatives.Count} derivative windows");
            Console.WriteLine();

            // Фаза 3: Определить базовую линию свободного навертывания
            var (baselineAvg, baselineStd) = CalculateBaseline(avgDerivatives);
            result.BaselineAverage = baselineAvg;
            result.BaselineStdDev = baselineStd;

            int startIdx = (int)(avgDerivatives.Count * 0.2);
            int endIdx = (int)(avgDerivatives.Count * 0.7);

            Console.WriteLine("Phase 2 (Free Threading) detected:");
            Console.WriteLine($"  Baseline average derivative: {baselineAvg:F2} Nm/ms");
            Console.WriteLine($"  Standard deviation: {baselineStd:F2} Nm/ms");
            Console.WriteLine($"  Torque range: {_series[windowCenters[startIdx]].Torque:F0}-{_series[windowCenters[endIdx]].Torque:F0} Nm");
            Console.WriteLine();

            // Фаза 4: Найти точку заплечника с настраиваемым порогом
            int searchStartIdx = (int)(avgDerivatives.Count * SearchStartRatio);
            result.SearchStartIndex = searchStartIdx;

            double threshold = baselineAvg + SigmaMultiplier * baselineStd;
            result.Threshold = threshold;

            int? shoulderWindowIdx = FindShoulderWindowIndex(avgDerivatives, baselineAvg, baselineStd);

            if (shoulderWindowIdx == null)
            {
                Console.WriteLine("ERROR: Shoulder point not found!");
                return result;
            }

            // Вернуть индекс в исходном массиве точек
            int shoulderIndex = windowCenters[shoulderWindowIdx.Value];
            result.ShoulderPointIndex = shoulderIndex;

            Console.WriteLine("=== SHOULDER POINT DETECTED ===");
            Console.WriteLine("Phase 3 (Shoulder Contact):");
            Console.WriteLine($"  Index: {shoulderIndex} of {_series.Count}");
            Console.WriteLine($"  Time: {_series[shoulderIndex].TimeStamp / 1000.0:F3} sec");
            Console.WriteLine($"  Turns: {_series[shoulderIndex].Turns:F5} turns");
            Console.WriteLine($"  Torque: {_series[shoulderIndex].Torque:F1} Nm");
            Console.WriteLine();
            Console.WriteLine($"  Average derivative at shoulder: {avgDerivatives[shoulderWindowIdx.Value]:F1} Nm/ms");
            Console.WriteLine($"  Baseline derivative: {baselineAvg:F1} Nm/ms");
            Console.WriteLine($"  Ratio: {avgDerivatives[shoulderWindowIdx.Value] / baselineAvg:F2}x");
            Console.WriteLine();

            return result;
        }

        private int? FindShoulderWindowIndex(List<double> avgDerivatives, double baselineAvg, double baselineStd)
        {
            int startIdx = (int)(avgDerivatives.Count * SearchStartRatio);

            // Используем настраиваемый множитель сигма
            double threshold = baselineAvg + SigmaMultiplier * baselineStd;

            Console.WriteLine($"Using {SigmaMultiplier}-sigma threshold: {threshold:F2} Nm/ms");
            Console.WriteLine($"Starting search from index {startIdx} ({SearchStartRatio * 100:F0}%)");

            // ВАЖНО: Ищем ПЕРВУЮ точку, превышающую порог, а не максимум!
            int? shoulderIdx = FindFirstDerivativeAboveThreshold(avgDerivatives, threshold, startIdx);

            if (shoulderIdx != null)
            {
                Console.WriteLine($"Shoulder found at window index {shoulderIdx}");
                Console.WriteLine($"Derivative at point: {avgDerivatives[shoulderIdx.Value]:F2} Nm/ms");

                // Покажем контекст вокруг найденной точки
                Console.WriteLine("Context (5 points before and after):");
                int contextStart = Math.Max(0, shoulderIdx.Value - 5);
                int contextEnd = Math.Min(avgDerivatives.Count, shoulderIdx.Value + 6);

                for (int i = contextStart; i < contextEnd; i++)
                {
                    string marker = i == shoulderIdx.Value ? " <-- SHOULDER" : "";
                    Console.WriteLine($"  Window {i}: derivative = {avgDerivatives[i]:F2} Nm/ms{marker}");
                }

                return shoulderIdx;
            }

            Console.WriteLine($"Shoulder not found with {SigmaMultiplier}-sigma threshold");
            return null;
        }

        /// <summary>
        /// Находит ПЕРВУЮ точку, где производная превышает порог
        /// (а не максимум, как было раньше!)
        /// </summary>
        private int? FindFirstDerivativeAboveThreshold(List<double> avgDerivatives, double threshold, int startFrom)
        {
            for (int i = startFrom; i < avgDerivatives.Count; i++)
            {
                if (avgDerivatives[i] > threshold)
                {
                    return i;
                }
            }

            return null;
        }

        // Остальные методы остаются без изменений
        private int FindMaxTorqueIndex()
        {
            float maxTorque = float.MinValue;
            int maxIndex = 0;

            for (int i = 0; i < _series.Count; i++)
            {
                if (_series[i].Torque > maxTorque)
                {
                    maxTorque = _series[i].Torque;
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        private (List<double> derivatives, List<int> centers) CalculateMovingAverageDerivatives(int analyzeEnd, List<double> smoothedTorque)
        {
            var rawDerivatives = new List<double>();
            var derivativeIndices = new List<int>();

            // Шаг 1: Вычислить "сырую" производную в каждой точке
            // Используем центральную разностную схему для более точного результата
            for (int i = 1; i < analyzeEnd - 1; i += Step)
            {
                // Центральная производная: (f(i+1) - f(i-1)) / (x(i+1) - x(i-1))
                double dTurns = _series[i + 1].Turns - _series[i - 1].Turns;

                if (dTurns > 0)
                {
                    double dTorque = smoothedTorque[i + 1] - smoothedTorque[i - 1];
                    double derivative = dTorque / dTurns; // Nm/оборот

                    rawDerivatives.Add(derivative);
                    derivativeIndices.Add(i);
                }
            }

            // Шаг 2: Сгладить производную через MovingAverage
            var smoothedDerivatives = new List<double>();
            var smoother = new MovingAverage(DerivativeWindowSize);

            foreach (var deriv in rawDerivatives)
            {
                double smoothed = smoother.SmoothValue(deriv);
                smoothedDerivatives.Add(smoothed);
            }

            return (smoothedDerivatives, derivativeIndices);
        }

        private (double average, double stdDev) CalculateBaseline(List<double> avgDerivatives)
        {
            int startIdx = (int)(avgDerivatives.Count * 0.2);
            int endIdx = (int)(avgDerivatives.Count * 0.7);

            var freeThreadingDerivatives = avgDerivatives.Skip(startIdx).Take(endIdx - startIdx).ToList();

            double baselineAvg = freeThreadingDerivatives.Average();

            double variance = 0;
            foreach (var d in freeThreadingDerivatives)
            {
                variance += Math.Pow(d - baselineAvg, 2);
            }
            double baselineStd = Math.Sqrt(variance / freeThreadingDerivatives.Count);

            return (baselineAvg, baselineStd);
        }

        /// <summary>
        /// Вычисляет сглаженные значения момента с использованием MovingAverage
        /// </summary>
        private List<double> CalculateSmoothedTorque()
        {
            var smoothedValues = new List<double>();

            if (_series == null || _series.Count == 0)
                return smoothedValues;

            var smoother = new MovingAverage(WindowSize);

            foreach (var point in _series)
            {
                double smoothed = smoother.SmoothValue(point.Torque);
                smoothedValues.Add(smoothed);
            }

            return smoothedValues;
        }
    }
}
