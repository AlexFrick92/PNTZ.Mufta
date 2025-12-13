using System.Collections.Generic;

namespace PNTZ.Mufta.TPCApp.Domain
{
    /// <summary>
    /// Результат работы детектора точки контакта с заплечником.
    /// Содержит найденную точку и промежуточные данные для визуализации алгоритма.
    /// </summary>
    public class ShoulderDetectionResult
    {
        /// <summary>
        /// Индекс точки контакта с заплечником в исходном массиве точек.
        /// Null, если точка не найдена.
        /// </summary>
        public int? ShoulderPointIndex { get; set; }

        /// <summary>
        /// Сглаженные производные момента по времени (Nm/ms).
        /// Вычисляются как скользящее среднее в окне WindowSize.
        /// </summary>
        public List<double> SmoothedDerivatives { get; set; }

        /// <summary>
        /// Индексы центров окон, соответствующие SmoothedDerivatives.
        /// Позволяют сопоставить производную с исходными точками.
        /// </summary>
        public List<int> WindowCenters { get; set; }

        /// <summary>
        /// Базовое среднее значение производной в фазе свободного навертывания (Nm/ms).
        /// Вычисляется на участке 20%-70% от общей длины.
        /// </summary>
        public double BaselineAverage { get; set; }

        /// <summary>
        /// Стандартное отклонение производной в фазе свободного навертывания (Nm/ms).
        /// </summary>
        public double BaselineStdDev { get; set; }

        /// <summary>
        /// Порог для определения точки заплечника (Nm/ms).
        /// Вычисляется как: BaselineAverage + SigmaMultiplier * BaselineStdDev
        /// </summary>
        public double Threshold { get; set; }

        /// <summary>
        /// Индекс начала области поиска заплечника (в массиве SmoothedDerivatives).
        /// Определяется параметром SearchStartRatio.
        /// </summary>
        public int SearchStartIndex { get; set; }

        /// <summary>
        /// Индекс максимального момента (конец фазы навертывания, начало разгрузки).
        /// </summary>
        public int MaxTorqueIndex { get; set; }

        /// <summary>
        /// Диапазон значений производной для нормализации (для визуализации).
        /// </summary>
        public double DerivativeMin { get; set; }

        /// <summary>
        /// Максимальное значение производной.
        /// </summary>
        public double DerivativeMax { get; set; }

        public ShoulderDetectionResult()
        {
            SmoothedDerivatives = new List<double>();
            WindowCenters = new List<int>();
        }
    }
}
