using LinqToDB.Mapping;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace PNTZ.Mufta.TPCApp.Repository
{
    [Table(Name = "Results")]
    public class JointResultTable : JointRecipeTable
    {

        [Column]
        public double MVS_Len { get; set; }
        [Column]
        public double FinalTorque { get; set; }
        [Column]
        public double FinalLength { get; set; }
        [Column]
        public double FinalJVal { get; set; }
        [Column]
        public double FinalTurns { get; set; }
        [Column]
        public double FinalShoulderTorque { get; set; }
        [Column]
        public double FinalShoulderTurns { get; set; }
        [Column]
        public long ResultPLC { get; set; }
        [Column]
        public long ResultTotal { get; set; }

        [Column]
        public string StartTimeStamp { get; set; }
        [Column]
        public string FinishTimeStamp { get; set; }

        [Column]
        public byte[] Series { get; set; }

        public JointResult ToJointResult()
        {
            JointResult result = new JointResult(base.ToJointRecipe());

            result.FinalTorque = (float)FinalTorque;
            result.FinalLength = (float)FinalLength;
            result.FinalJVal = (float)FinalJVal;
            result.FinalTurns = (float)FinalTurns;
            result.FinalShoulderTurns = (float)FinalShoulderTurns;
            result.FinalShoulderTorque = (float)FinalShoulderTorque;
            result.ResultPLC = (uint)ResultPLC;
            result.ResultTotal = (uint)ResultTotal;
            result.StartTimeStamp = DateTime.Parse(StartTimeStamp);
            result.FinishTimeStamp = DateTime.Parse(FinishTimeStamp);
            result.MVS_Len = (float)MVS_Len;

            using (MemoryStream ms = new MemoryStream(Series))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                result.Series = new List<TqTnLenPoint>();
                foreach (var point in (TqTnLenPoint[])formatter.Deserialize(ms))
                {
                    result.Series.Add(point);
                }

                result.CalculateTurnPerMinute();

            }
           
            return result;  

        }

        public JointResultTable FromJointResult(JointResult result)
        {
            if (result.Recipe != null)
                base.FromJointRecipe(result.Recipe);
            else
                base.FromJointRecipe(new JointRecipe() { Name = "Рецепт не задан"});

            FinalTorque = result.FinalTorque;
            FinalLength = result.FinalLength;
            FinalJVal = result.FinalJVal;
            FinalTurns = result.FinalTurns;
            FinalShoulderTorque = result.FinalShoulderTorque;
            FinalShoulderTurns = result.FinalShoulderTurns;
            ResultPLC = result.ResultPLC;
            ResultTotal = result.ResultTotal;
            StartTimeStamp = result.StartTimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
            FinishTimeStamp = result.FinishTimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
            MVS_Len = result.MVS_Len;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, result.Series.ToArray());
                Series = ms.ToArray();
            }

            return this;
        }
    }
}
