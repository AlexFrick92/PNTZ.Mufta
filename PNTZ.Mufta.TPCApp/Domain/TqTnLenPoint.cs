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
    }
}
