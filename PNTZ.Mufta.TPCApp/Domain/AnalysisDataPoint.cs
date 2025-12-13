using System;

namespace PNTZ.Mufta.TPCApp.Domain
{
    /// <summary>
    /// Расширенная точка данных для анализа результатов свинчивания
    /// Содержит дополнительные вычисленные параметры помимо базовых данных
    /// </summary>
    [Serializable]
    public class AnalysisDataPoint : TqTnLenPoint
    {
        /// <summary>
        /// Сглаженное значение момента
        /// </summary>
        public float SmoothedTorque { get; set; }

        /// <summary>
        /// Первая производная момента по оборотам (резерв для будущей реализации)
        /// </summary>
        public float TorqueDerivative { get; set; }

        /// <summary>
        /// Создает AnalysisDataPoint на основе базовой точки TqTnLenPoint
        /// </summary>
        public static AnalysisDataPoint FromTqTnLenPoint(TqTnLenPoint point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));

            return new AnalysisDataPoint
            {
                Torque = point.Torque,
                Length = point.Length,
                Turns = point.Turns,
                TurnsPerMinute = point.TurnsPerMinute,
                TimeStamp = point.TimeStamp,
                SmoothedTorque = point.Torque, // Пока без сглаживания
                TorqueDerivative = 0f // Пока не вычисляется
            };
        }
    }
}
