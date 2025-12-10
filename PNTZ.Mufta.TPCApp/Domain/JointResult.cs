
using System;
using System.Collections.Generic;

namespace PNTZ.Mufta.TPCApp.Domain
{
    public class JointResult
    {
        public JointResult(JointRecipe recipe)
        {
            Recipe = recipe;
        }
        public JointRecipe Recipe { get; private set; }
        public float MVS_Len { get; set; }
        public float MVS_Len_mm => MVS_Len * 1000;
        public float FinalTorque { get; set; }
        public float FinalLength { get; set; }
        public float FinalLength_mm => FinalLength * 1000;
        public float FinalMakeupLength_mm => (FinalLength - MVS_Len) * 1000;
        public float FinalJVal { get; set; }
        public float FinalTurns { get; set; }
        public float FinalShoulderTorque { get; set; }
        public float FinalShoulderTurns { get; set; }
        public uint ResultPLC { get; set; }
        public uint ResultTotal { get; set; }
        public DateTime StartTimeStamp { get; set; }
        public DateTime FinishTimeStamp { get; set; }
        public List<TqTnLenPoint> Series { get; set; } = new List<TqTnLenPoint>();        
    }
}
