using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpConnect;

namespace PNTZ.Mufta.TPCApp.DpConnect
{
    public class SensorStatusDpWorker : IDpWorker
    {

        public IDpValue<bool> DpAED { get; set; }
        public IDpValue<bool> DpRotate { get; set; }
        public IDpValue<bool> DpLength { get; set; }

        public void DpBound()
        {
            
            
        }

    }
}
