using System;
using System.Collections.Generic;

namespace PNTZ.Mufta.Showcase.Data
{
    /// <summary>
    /// Точка данных для графика крутящего момента
    /// </summary>
    public class SeriesPoint
    {
        public int XVal { get; set; }
        public double YVal1 { get; set; }
        public double YVal2 { get; set; }
        public double YVal3 { get; set; }
        public double YVal4 { get; set; }

    }

    /// <summary>
    /// Генератор тестовых данных для контролов
    /// </summary>
    public static class MockDataGenerator
    {
        private static Random _random = new Random();

        /// <summary>
        /// Генерирует реалистичные данные для графика крутящего момента
        /// Имитирует реальный процесс навёртки муфты
        /// </summary>
        /// <param name="xValues">Массив значений X (время в мс)</param>
        /// <param name="maxTorque">Максимальный крутящий момент</param>
        /// <returns>Массив значений Y (крутящий момент)</returns>
        public static double[] GenerateRealisticTorqueData(int[] xValues, double maxTorque = 8000)
        {
            int pointCount = xValues.Length;
            double[] yValues = new double[pointCount];

            // Случайные параметры для каждой генерации
            double phase1End = 0.15 + _random.NextDouble() * 0.15;      // 15-30%
            double phase2End = 0.50 + _random.NextDouble() * 0.25;      // 50-75%
            double phase1Peak = 0.30 + _random.NextDouble() * 0.20;     // 30-50% от максимума
            double phase2Level = 0.40 + _random.NextDouble() * 0.20;    // 40-60% от максимума
            double noiseLevel = 0.02 + _random.NextDouble() * 0.03;     // 2-5% шума
            double growthCurve = 1.2 + _random.NextDouble() * 1.3;      // 1.2-2.5 экспонента роста

            // Генерация случайных "событий" (резкие изменения)
            int eventsCount = _random.Next(2, 6);
            List<int> eventIndices = new List<int>();
            List<double> eventMagnitudes = new List<double>();

            for (int e = 0; e < eventsCount; e++)
            {
                int eventIndex = _random.Next((int)(pointCount * 0.1), (int)(pointCount * 0.9));
                double eventMagnitude = (_random.NextDouble() - 0.5) * 0.15; // ±15%
                eventIndices.Add(eventIndex);
                eventMagnitudes.Add(eventMagnitude);
            }

            for (int i = 0; i < pointCount; i++)
            {
                double progress = (double)i / pointCount;
                double torque;

                if (progress < phase1End)
                {
                    // Фаза 1: Начальный подъем с вариацией
                    double phase1Progress = progress / phase1End;
                    torque = Math.Pow(phase1Progress, growthCurve) * maxTorque * phase1Peak;

                    // Добавляем волнообразность в начале
                    torque += Math.Sin(phase1Progress * Math.PI * 4) * maxTorque * 0.05;
                }
                else if (progress < phase2End)
                {
                    // Фаза 2: Стабилизация с колебаниями и трендом
                    double phase2Progress = (progress - phase1End) / (phase2End - phase1End);
                    double baseLevel = maxTorque * phase2Level;

                    // Медленный рост внутри фазы
                    double trend = phase2Progress * maxTorque * 0.15;

                    // Волнообразные колебания разной частоты
                    double wave1 = Math.Sin(phase2Progress * Math.PI * 8) * maxTorque * 0.08;
                    double wave2 = Math.Sin(phase2Progress * Math.PI * 3) * maxTorque * 0.05;

                    torque = baseLevel + trend + wave1 + wave2;
                }
                else
                {
                    // Фаза 3: Финальный рост к целевому моменту
                    double phase3Progress = (progress - phase2End) / (1.0 - phase2End);
                    double startLevel = maxTorque * phase2Level;
                    double growthRange = maxTorque - startLevel;

                    // Нелинейный рост с вариацией
                    torque = startLevel + Math.Pow(phase3Progress, growthCurve * 0.8) * growthRange;

                    // Добавляем финальные колебания
                    torque += Math.Sin(phase3Progress * Math.PI * 5) * maxTorque * 0.04;
                }

                // Применяем случайные "события"
                for (int e = 0; e < eventIndices.Count; e++)
                {
                    int eventIndex = eventIndices[e];
                    double eventMagnitude = eventMagnitudes[e];

                    // Гауссово влияние события (влияет на окрестность точки)
                    double distance = Math.Abs(i - eventIndex);
                    double eventWidth = pointCount * 0.05; // 5% ширина события
                    double influence = Math.Exp(-Math.Pow(distance / eventWidth, 2));

                    torque += influence * eventMagnitude * maxTorque;
                }

                // Добавляем высокочастотный шум
                double noise = (_random.NextDouble() - 0.5) * maxTorque * noiseLevel;

                // Добавляем низкочастотный шум (Perlin-подобный)
                double lowFreqNoise = Math.Sin(progress * Math.PI * 20 + _random.NextDouble() * 2) * maxTorque * (noiseLevel * 0.5);

                torque += noise + lowFreqNoise;

                // Ограничиваем значения
                torque = Math.Max(0, Math.Min(maxTorque * 1.1, torque));

                yValues[i] = torque;
            }

            // Применяем легкое сглаживание для более естественного вида
            yValues = ApplyMovingAverage(yValues, 3);

            return yValues;
        }

        /// <summary>
        /// Применяет скользящее среднее для сглаживания данных
        /// </summary>
        private static double[] ApplyMovingAverage(double[] data, int windowSize)
        {
            if (windowSize <= 1 || data.Length < windowSize)
                return data;

            double[] smoothed = new double[data.Length];
            int halfWindow = windowSize / 2;

            for (int i = 0; i < data.Length; i++)
            {
                double sum = 0;
                int count = 0;

                for (int j = Math.Max(0, i - halfWindow); j <= Math.Min(data.Length - 1, i + halfWindow); j++)
                {
                    sum += data[j];
                    count++;
                }

                smoothed[i] = sum / count;
            }

            return smoothed;
        }

        /// <summary>
        /// Генерирует синусоидальные данные для тестирования
        /// </summary>
        /// <param name="xValues">Массив значений X (время в мс)</param>
        /// <param name="amplitude">Амплитуда синусоиды</param>
        /// <param name="frequency">Частота синусоиды (количество периодов)</param>
        /// <returns>Массив значений Y</returns>
        public static double[] GenerateSineWaveData(int[] xValues, double amplitude = 5000, double frequency = 3)
        {
            int pointCount = xValues.Length;
            double[] yValues = new double[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                yValues[i] = amplitude * Math.Sin(frequency * 2 * Math.PI * i / pointCount) + amplitude;
            }

            return yValues;
        }

        /// <summary>
        /// Генерирует случайные данные
        /// </summary>
        /// <param name="xValues">Массив значений X (время в мс)</param>
        /// <param name="maxTorque">Максимальный крутящий момент</param>
        /// <returns>Массив значений Y</returns>
        public static double[] GenerateRandomData(int[] xValues, double maxTorque = 10000)
        {
            int pointCount = xValues.Length;
            double[] yValues = new double[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                yValues[i] = maxTorque * _random.NextDouble();
            }

            return yValues;
        }

        /// <summary>
        /// Генерирует линейно возрастающие данные
        /// </summary>
        /// <param name="xValues">Массив значений X (время в мс)</param>
        /// <param name="maxTorque">Максимальный крутящий момент</param>
        /// <returns>Массив значений Y</returns>
        public static double[] GenerateLinearData(int[] xValues, double maxTorque = 10000)
        {
            int pointCount = xValues.Length;
            double[] yValues = new double[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                yValues[i] = (maxTorque / pointCount) * i;
            }

            return yValues;
        }
    }
}
