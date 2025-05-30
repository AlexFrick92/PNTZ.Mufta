using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class TqTnLenPointViewModel : BaseViewModel
    {
        readonly TqTnLenPoint point;
        public TqTnLenPointViewModel(TqTnLenPoint point)
        {
            this.point = point;
        }
        public float Torque => Math.Abs(point.Torque);

        /// <summary>
        /// В миллиметрах
        /// </summary>
        /// 
        public float Length => point.Length * 1000;

        public float Turns => point.Turns;
        public int TimeStamp => point.TimeStamp;

        public float TurnsPerMinute => point.TurnsPerMinute;
    }
}
