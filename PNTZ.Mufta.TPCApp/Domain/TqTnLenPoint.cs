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
        public float Turns { get; set; }

        [NonSerialized]
        private float _turnsPerMinute;

        public float TurnsPerMinute
        {
            get => _turnsPerMinute;
            set => _turnsPerMinute = value;
        }


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
    }
}
