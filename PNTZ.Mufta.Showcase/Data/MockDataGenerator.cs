using System;
using System.Collections.Generic;

namespace PNTZ.Mufta.Showcase.Data
{
    /// <summary>
    /// Точка данных для графика крутящего момента
    /// </summary>
    public class TorquePoint
    {
        public double Turns { get; set; }
        public double Torque { get; set; }
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
        /// <param name="pointCount">Количество точек</param>
        /// <param name="maxTurns">Максимальное количество оборотов</param>
        /// <param name="maxTorque">Максимальный крутящий момент</param>
        /// <returns>Список точек для графика</returns>
        public static List<TorquePoint> GenerateRealisticTorqueData(
            int pointCount = 100,
            double maxTurns = 50,
            double maxTorque = 8000)
        {
            var data = new List<TorquePoint>();

            for (int i = 0; i < pointCount; i++)
            {
                double turns = (maxTurns / pointCount) * i;

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

                data.Add(new TorquePoint { Turns = turns, Torque = torque });
            }

            return data;
        }

        /// <summary>
        /// Генерирует синусоидальные данные для тестирования
        /// </summary>
        /// <param name="pointCount">Количество точек</param>
        /// <param name="minTurns">Минимальное значение оборотов</param>
        /// <param name="maxTurns">Максимальное значение оборотов</param>
        /// <param name="amplitude">Амплитуда синусоиды</param>
        /// <param name="frequency">Частота синусоиды</param>
        /// <returns>Список точек для графика</returns>
        public static List<TorquePoint> GenerateSineWaveData(
            int pointCount = 200,
            double minTurns = 0,
            double maxTurns = 100,
            double amplitude = 5000,
            double frequency = 2)
        {
            var data = new List<TorquePoint>();
            double turnsRange = maxTurns - minTurns;

            for (int i = 0; i < pointCount; i++)
            {
                double turns = minTurns + (turnsRange / pointCount) * i;
                double torque = amplitude * Math.Sin(frequency * Math.PI * i / pointCount) + amplitude;

                data.Add(new TorquePoint { Turns = turns, Torque = torque });
            }

            return data;
        }

        /// <summary>
        /// Генерирует случайные данные
        /// </summary>
        /// <param name="pointCount">Количество точек</param>
        /// <param name="maxTurns">Максимальное количество оборотов</param>
        /// <param name="maxTorque">Максимальный крутящий момент</param>
        /// <returns>Список точек для графика</returns>
        public static List<TorquePoint> GenerateRandomData(
            int pointCount = 100,
            double maxTurns = 50,
            double maxTorque = 10000)
        {
            var data = new List<TorquePoint>();

            for (int i = 0; i < pointCount; i++)
            {
                data.Add(new TorquePoint
                {
                    Turns = (maxTurns / pointCount) * i,
                    Torque = maxTorque * _random.NextDouble()
                });
            }

            return data;
        }

        /// <summary>
        /// Генерирует линейно возрастающие данные
        /// </summary>
        /// <param name="pointCount">Количество точек</param>
        /// <param name="maxTurns">Максимальное количество оборотов</param>
        /// <param name="maxTorque">Максимальный крутящий момент</param>
        /// <returns>Список точек для графика</returns>
        public static List<TorquePoint> GenerateLinearData(
            int pointCount = 100,
            double maxTurns = 50,
            double maxTorque = 10000)
        {
            var data = new List<TorquePoint>();

            for (int i = 0; i < pointCount; i++)
            {
                double turns = (maxTurns / pointCount) * i;
                double torque = (maxTorque / pointCount) * i;

                data.Add(new TorquePoint { Turns = turns, Torque = torque });
            }

            return data;
        }
    }
}
