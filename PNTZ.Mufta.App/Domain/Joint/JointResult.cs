using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.App.Domain.Joint
{
    public class JointResult
    {
        public float FinalTq { get; set; }
        public float FinalLen { get; set; }
        public float FinalJVal { get; set; }
        public float FinalTurns { get; set; }
        public float FinalShoulderTq { get; set; }
        public float FinalShoulderTurns { get; set; }
        public uint ResultPLC { get; set; } 
        public uint ResultTotal { get; set; }        
        public DateTime StartTimeStamp {  get; set; }
        public DateTime FinishTimeStamp { get; set; }

        public List<TqTnPoint> TqTnPoints { get; private set; } = new List<TqTnPoint>();

    }
}
