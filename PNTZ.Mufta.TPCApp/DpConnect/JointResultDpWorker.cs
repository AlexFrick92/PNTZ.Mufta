using DpConnect;
using PNTZ.Mufta.TPCApp.DpConnect.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.DpConnect
{
    public class JointResultDpWorker : IDpWorker
    {
        public IDpValue<uint> DpTpcCommand { get; set; }
        public IDpValue<uint> DpPlcCommand { get; set; }
        public IDpValue<ERG_CAM> Dp_ERG_CAM { get; set; }
        public IDpValue<ERG_Muffe> Dp_ERG_Muffe { get; set; }
        public IDpValue<ERG_MVS> Dp_ERG_MVS { get; set; }


        public void DpBound()
        {
            
        }
    }
}
