
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
        public List<TqTnLenPoint> Series { get; set; } = new List<TqTnLenPoint>();

        public void CalculateTurnPerMinute()
        {
            if (Series.Count < 2)
            {
                throw new Exception();
                return;
            }

            for (int i = 1; i < Series.Count; i++)
            {
                Series[i - 1].TurnsPerMinute = (float)TqTnLenPoint.CalculateTurnsPerMinute(Series[i - 1], Series[i]);
            }
        }
    }
}
