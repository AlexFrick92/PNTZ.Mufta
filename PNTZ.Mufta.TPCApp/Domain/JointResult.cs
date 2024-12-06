
using System;
using System.Collections.Generic;

namespace PNTZ.Mufta.TPCApp.Domain
{
    public class JointResult
    {
        public float FinalTorque { get; set; }
        public float FinalLength { get; set; }
        public float FinalJVal { get; set; }
        public float FinalTurns { get; set; }
        public float FinalShoulderTorque { get; set; }
        public float FinalShoulderTurns { get; set; }
        public uint ResultPLC { get; set; }
        public uint ResultTotal { get; set; }
        public DateTime StartTimeStamp { get; set; }
        public DateTime FinishTimeStamp { get; set; }
        public List<TqTnLenPoint> Series { get; private set; }
    }
}
