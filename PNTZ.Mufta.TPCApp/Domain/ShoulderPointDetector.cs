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
        /// Размер окна для скользящего усреднения производной (количество точек)
        /// </summary>
        public int WindowSize { get; set; } = 200;

        /// <summary>
        /// Шаг для вычисления производной (каждая N-ая точка)
        /// </summary>
        public int Step { get; set; } = 5;

        public ShoulderPointDetector(List<TqTnLenPoint> series)
        {
            _series = series ?? throw new ArgumentNullException(nameof(series));
        }

        /// <summary>
        /// Определяет индекс точки контакта с заплечником.
        /// Использует анализ скользящего среднего производной момента по времени.
        /// </summary>
        /// <returns>Индекс точки контакта или null, если точка не найдена</returns>
        public int? DetectShoulderPoint()
        {
            Console.WriteLine("=== Universal Shoulder Detection ===");
            Console.WriteLine($"Total points: {_series.Count}");
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

            // Фаза 2: Вычислить скользящее среднее производной dTorque/dTime
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

            // Фаза 4: Найти точку заплечника (превышение порога 3-сигма или 2-сигма)
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

        /// <summary>
        /// Находит индекс точки с максимальным моментом
        /// </summary>
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

        /// <summary>
        /// Вычисляет скользящее среднее производной момента по времени
        /// </summary>
        /// <param name="analyzeEnd">Конечный индекс для анализа</param>
        /// <returns>Кортеж: массив производных и соответствующих им центров окон</returns>
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
                    // Производная dTorque/dTime
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

        /// <summary>
        /// Вычисляет базовую линию производной для фазы свободного навертывания
        /// </summary>
        /// <param name="avgDerivatives">Массив производных</param>
        /// <returns>Кортеж: среднее значение и стандартное отклонение</returns>
        private (double average, double stdDev) CalculateBaseline(List<double> avgDerivatives)
        {
            // Берем средние 50% данных (от 20% до 70%)
            int startIdx = (int)(avgDerivatives.Count * 0.2);
            int endIdx = (int)(avgDerivatives.Count * 0.7);

            var freeThreadingDerivatives = avgDerivatives.Skip(startIdx).Take(endIdx - startIdx).ToList();

            double baselineAvg = freeThreadingDerivatives.Average();

            // Вычисляем стандартное отклонение
            double variance = 0;
            foreach (var d in freeThreadingDerivatives)
            {
                variance += Math.Pow(d - baselineAvg, 2);
            }
            double baselineStd = Math.Sqrt(variance / freeThreadingDerivatives.Count);

            return (baselineAvg, baselineStd);
        }

        /// <summary>
        /// Находит индекс окна, где производная превышает порог (3-сигма или 2-сигма)
        /// </summary>
        /// <param name="avgDerivatives">Массив производных</param>
        /// <param name="baselineAvg">Базовое среднее</param>
        /// <param name="baselineStd">Базовое стандартное отклонение</param>
        /// <returns>Индекс окна или null</returns>
        private int? FindShoulderWindowIndex(List<double> avgDerivatives, double baselineAvg, double baselineStd)
        {
            int endIdx = (int)(avgDerivatives.Count * 0.7);

            // Пробуем правило 3-сигма
            double threshold = baselineAvg + 3 * baselineStd;
            Console.WriteLine($"Trying 3-sigma threshold: {threshold:F2} Nm/ms");
            int? shoulderIdx = FindFirstExceedingThreshold(avgDerivatives, threshold, endIdx);

            if (shoulderIdx != null)
            {
                Console.WriteLine($"Shoulder found with 3-sigma at window index {shoulderIdx}");
                return shoulderIdx;
            }

            // Если не найдено, пробуем 2-сигма
            Console.WriteLine("Shoulder not found with 3-sigma threshold");
            Console.WriteLine("Trying 2-sigma threshold...");
            threshold = baselineAvg + 2 * baselineStd;
            Console.WriteLine($"2-sigma threshold: {threshold:F2} Nm/ms");
            shoulderIdx = FindFirstExceedingThreshold(avgDerivatives, threshold, endIdx);

            if (shoulderIdx != null)
            {
                Console.WriteLine($"Shoulder found with 2-sigma at window index {shoulderIdx}");
            }

            return shoulderIdx;
        }

        /// <summary>
        /// Находит первый индекс, где производная превышает порог
        /// </summary>
        private int? FindFirstExceedingThreshold(List<double> avgDerivatives, double threshold, int startFrom)
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
    }
}
