using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.App.Domain.Joint
{
    public class TqTnPoint
    {
        public float Tq { get; set; }
        public float Tn { get; set; }
        public int TimeStamp { get; set; }

        public override string ToString()
        {
            return Tq.ToString() + " : " + Tn.ToString();
        }
    }
}
