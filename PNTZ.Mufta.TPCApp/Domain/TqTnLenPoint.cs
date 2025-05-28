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
        public float TurnsPerMinute { get; set; }
        public int TimeStamp { get; set; }
    }
}
