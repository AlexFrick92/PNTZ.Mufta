using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Размер окна для сглаживания (борьба с шумом)
        /// </summary>
        public int WindowSize { get; set; } = 20;

        /// <summary>
        /// Множитель сигма для определения порога (чувствительность детектора)
        /// Меньше = раньше срабатывает, больше = позже срабатывает
        /// </summary>
        public double SigmaMultiplier { get; set; } = 7;

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

        public int? DetectShoulderPoint()
        {
            Console.WriteLine("=== Universal Shoulder Detection ===");
            Console.WriteLine($"Total points: {_series.Count}");
            Console.WriteLine($"Parameters:");
            Console.WriteLine($"  WindowSize (smoothing): {WindowSize}");
            Console.WriteLine($"  SigmaMultiplier (sensitivity): {SigmaMultiplier}");
            Console.WriteLine($"  SearchStartRatio: {SearchStartRatio}");
            Console.WriteLine();

            if (_series == null || _series.Count < WindowSize * 2)
            {
                Console.WriteLine("ERROR: Not enough points for analysis");
                return null;
            }

            // Фаза 1: Найти максимум момента и отсечь фазу разгрузки
            int maxIndex = FindMaxTorqueIndex();
            int analyzeEnd = maxIndex;

            float maxTorque = _series[maxIndex].Torque;
            Console.WriteLine($"Phase 4 (Unloading): Max torque {maxTorque:F1} Nm at index {maxIndex}");
            Console.WriteLine("Analyzing up to maximum, excluding unloading phase");
            Console.WriteLine();

            // Фаза 2: Вычислить скользящее среднее производной
            Console.WriteLine($"Calculating moving average derivatives (window={WindowSize} points)...");
            var (avgDerivatives, windowCenters) = CalculateMovingAverageDerivatives(analyzeEnd);

            if (avgDerivatives.Count == 0)
            {
                Console.WriteLine("ERROR: No derivatives calculated");
                return null;
            }

            Console.WriteLine($"Calculated {avgDerivatives.Count} windows");
            Console.WriteLine();

            // Фаза 3: Определить базовую линию свободного навертывания
            var (baselineAvg, baselineStd) = CalculateBaseline(avgDerivatives);

            int startIdx = (int)(avgDerivatives.Count * 0.2);
            int endIdx = (int)(avgDerivatives.Count * 0.7);

            Console.WriteLine("Phase 2 (Free Threading) detected:");
            Console.WriteLine($"  Baseline average derivative: {baselineAvg:F2} Nm/ms");
            Console.WriteLine($"  Standard deviation: {baselineStd:F2} Nm/ms");
            Console.WriteLine($"  Torque range: {_series[windowCenters[startIdx]].Torque:F0}-{_series[windowCenters[endIdx]].Torque:F0} Nm");
            Console.WriteLine();

            // Фаза 4: Найти точку заплечника с настраиваемым порогом
            int? shoulderWindowIdx = FindShoulderWindowIndex(avgDerivatives, baselineAvg, baselineStd);

            if (shoulderWindowIdx == null)
            {
                Console.WriteLine("ERROR: Shoulder point not found!");
                return null;
            }

            // Вернуть индекс в исходном массиве точек
            int shoulderIndex = windowCenters[shoulderWindowIdx.Value];

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

            return shoulderIndex;
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

        private (List<double> derivatives, List<int> centers) CalculateMovingAverageDerivatives(int analyzeEnd)
        {
            var avgDerivatives = new List<double>();
            var windowCenters = new List<int>();

            int halfWindow = WindowSize / 2;

            for (int i = WindowSize; i < analyzeEnd - WindowSize; i += Step)
            {
                double sum = 0;
                int count = 0;

                for (int j = i - halfWindow; j < i + halfWindow - 1; j++)
                {
                    double dt = _series[j + 1].TimeStamp - _series[j].TimeStamp;

                    if (dt > 0)
                    {
                        double dTorque = _series[j + 1].Torque - _series[j].Torque;
                        sum += dTorque / dt;
                        count++;
                    }
                }

                if (count > 0)
                {
                    avgDerivatives.Add(sum / count);
                    windowCenters.Add(i);
                }
            }

            return (avgDerivatives, windowCenters);
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
    }
}
