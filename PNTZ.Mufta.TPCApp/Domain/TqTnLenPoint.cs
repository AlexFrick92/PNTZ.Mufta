using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Domain
{
    [Serializable]
    public class TqTnLenPoint
    {
        public float Torque { get; set; }
        /// <summary>
        /// В миллиметрах
        /// </summary>
        public float Length { get; set; }
        /// <summary>
        /// Длина в миллиметрах (псевдоним для Length)
        /// </summary>
        public float Length_mm => Length;
        public float Turns { get; set; }

        public float TurnsPerMinute { get; set; }


        public int TimeStamp { get; set; }

        public static TqTnLenPoint SmoothAverage(IList<TqTnLenPoint> points)
        {
            if (points == null || points.Count == 0)
                throw new ArgumentException("Points buffer is empty");

            var count = points.Count;

            var avgTorque = points.Average(p => p.Torque);
            var avgLength = points.Average(p => p.Length);
            var avgTurns = points.Average(p => p.Turns);
            var avgTpm = points.Average(p => p.TurnsPerMinute);

            // TimeStamp берём у последнего элемента
            var lastTimeStamp = points.Last().TimeStamp;

            return new TqTnLenPoint
            {
                Torque = avgTorque,
                Length = avgLength,
                Turns = avgTurns,
                TurnsPerMinute = avgTpm,
                TimeStamp = lastTimeStamp
            };
        }
    }
}
