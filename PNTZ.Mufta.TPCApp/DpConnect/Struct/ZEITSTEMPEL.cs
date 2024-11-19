using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.DpConnect.Struct
{
    public class ZEITSTEMPEL
    {
        public DateTime GENERAL_TS { get; set; }
        public DateTime BOX_TS { get; set; }
        public DateTime PREMAKEUP_TS { get; set; }        
        public DateTime MAKEUP_TS { get; set; }
    }
}
