using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.DpConnect.Struct
{
    public class OperationalParam
    {
        /// <summary>
        /// В метрах
        /// </summary>
        public float Length { get; set; }
        /// <summary>
        /// Количество
        /// </summary>
        public float Turns { get; set; }
        /// <summary>
        /// В ньютонах
        /// </summary>
        public float Torque { get; set; }

        /// <summary>
        /// Обороты в минуту
        /// </summary>
        public float TurnsPerMinute { get; set; }
    }
}
