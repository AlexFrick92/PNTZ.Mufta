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
        TqTnLenPoint point;
        public TqTnLenPointViewModel(TqTnLenPoint point)
        {
            this.point = point;
        }
        public float Torque { get => Math.Abs(point.Torque); }

        /// <summary>
        /// В миллиметрах
        /// </summary>
        /// 
        public float Length { get => point.Length * 1000; }
        public float Turns { get => point.Turns; }
        public int TimeStamp { get => point.TimeStamp; }

        public float TurnsPerMinute { get; set; }

        
    }
}
