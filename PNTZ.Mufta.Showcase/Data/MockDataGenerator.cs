using System;
using System.Collections.Generic;

namespace PNTZ.Mufta.Showcase.Data
{
    /// <summary>
    /// Точка данных для графика крутящего момента
    /// </summary>
    public class SeriesPoint
    {
        public double XVal { get; set; }
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
        public static double[] GenerateRealisticTorqueData(double[] xValues, double maxTorque = 8000)
        {
            int pointCount = xValues.Length;
            double[] yValues = new double[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                // Симуляция реального процесса навёртки
                // Фаза 1: Начальный подъем (0-20%) - быстрый рост момента
                // Фаза 2: Стабилизация (20-60%) - момент держится на уровне с колебаниями
                // Фаза 3: Финальный рост (60-100%) - момент растет к финальному значению

                double torque;
                if (i < pointCount * 0.2)
                {
                    // Фаза 1: Начальный подъем
                    torque = Math.Pow(i / (pointCount * 0.2), 1.5) * maxTorque * 0.4;
                }
                else if (i < pointCount * 0.6)
                {
                    // Фаза 2: Стабилизация с небольшими колебаниями
                    torque = maxTorque * 0.4 + (maxTorque * 0.2) * _random.NextDouble();
                }
                else
                {
                    // Фаза 3: Финальный рост
                    double progress = (i - pointCount * 0.6) / (pointCount * 0.4);
                    torque = maxTorque * 0.6 + Math.Pow(progress, 2) * maxTorque * 0.4;
                }

                // Добавляем небольшой шум (±2% от максимума)
                torque += (_random.NextDouble() - 0.5) * maxTorque * 0.02;

                yValues[i] = torque;
            }

            return yValues;
        }

        /// <summary>
        /// Генерирует синусоидальные данные для тестирования
        /// </summary>
        /// <param name="xValues">Массив значений X (время в мс)</param>
        /// <param name="amplitude">Амплитуда синусоиды</param>
        /// <param name="frequency">Частота синусоиды (количество периодов)</param>
        /// <returns>Массив значений Y</returns>
        public static double[] GenerateSineWaveData(double[] xValues, double amplitude = 5000, double frequency = 3)
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
        public static double[] GenerateRandomData(double[] xValues, double maxTorque = 10000)
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
        public static double[] GenerateLinearData(double[] xValues, double maxTorque = 10000)
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
