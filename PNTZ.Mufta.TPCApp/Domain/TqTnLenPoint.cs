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

        public static double CalculateTurnsPerMinute(TqTnLenPoint lastPoint, TqTnLenPoint newPoint)
        {
            if (lastPoint == null || newPoint == null)
                return 0;

            const int millisecondsInMinute = 60_000;

            double dV = (newPoint.Turns - lastPoint.Turns);
            double dT = (newPoint.TimeStamp - lastPoint.TimeStamp);
            double dTminutes = dT / millisecondsInMinute;
            double changeRate = (dV / dTminutes);


            if (dTminutes > 0 && dV != 0)
                return (float)changeRate;
            else
                return lastPoint.TurnsPerMinute;
        }

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
