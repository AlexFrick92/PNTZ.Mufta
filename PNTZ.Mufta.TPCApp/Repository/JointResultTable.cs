using LinqToDB.Mapping;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using LinqToDB;

namespace PNTZ.Mufta.TPCApp.Repository
{
    [Table(Name = "Results")]
    public class JointResultTable : JointRecipeTable
    {
        public JointResultTable()
        {
            
        }
        public JointResultTable(JointRecipeTable recipe)
        {
            if (recipe == null) throw new ArgumentNullException("Recipe is Null");

            Name = recipe.Name;
            HEAD_OPEN_PULSES = recipe.HEAD_OPEN_PULSES;
            TURNS_BREAK = recipe.TURNS_BREAK;
            PLC_PROG_NR = recipe.PLC_PROG_NR;
            LOG_NO = recipe.LOG_NO;
            Tq_UNIT = recipe.Tq_UNIT;
            SelectedThreadType = (int)recipe.SelectedThreadType;
            Thread_step = recipe.Thread_step;
            PIPE_TYPE = recipe.PIPE_TYPE;

            Box_Moni_Time = recipe.Box_Moni_Time;
            Box_Len_Min = recipe.Box_Len_Min;
            Box_Len_Max = recipe.Box_Len_Max;

            Pre_Moni_Time = recipe.Pre_Moni_Time;
            Pre_Len_Max = recipe.Pre_Len_Max;
            Pre_Len_Min = recipe.Pre_Len_Min;

            MU_Moni_Time = recipe.MU_Moni_Time;
            MU_Tq_Ref = recipe.MU_Tq_Ref;
            MU_Tq_Save = recipe.MU_Tq_Save;

            SelectedMode = (int)recipe.JointMode;

            MU_TqSpeedRed_1 = recipe.MU_TqSpeedRed_1;
            MU_TqSpeedRed_2 = recipe.MU_TqSpeedRed_2;
            MU_Tq_Dump = recipe.MU_Tq_Dump;
            MU_Tq_Max = recipe.MU_Tq_Max;
            MU_Tq_Min = recipe.MU_Tq_Min;
            MU_Tq_Opt = recipe.MU_Tq_Opt;

            MU_TqShoulder_Min = recipe.MU_TqShoulder_Min;
            MU_TqShoulder_Max = recipe.MU_TqShoulder_Max;

            MU_Len_Speed_1 = recipe.MU_Len_Speed_1;
            MU_Len_Speed_2 = recipe.MU_Len_Speed_2;
            MU_Len_Dump = recipe.MU_Len_Dump;
            MU_Len_Min = recipe.MU_Len_Min;
            MU_Len_Max = recipe.MU_Len_Max;

            MU_JVal_Speed_1 = recipe.MU_JVal_Speed_1;
            MU_JVal_Speed_2 = recipe.MU_JVal_Speed_2;
            MU_JVal_Dump = recipe.MU_JVal_Dump;
            MU_JVal_Min = recipe.MU_JVal_Min;
            MU_JVal_Max = recipe.MU_JVal_Max;

            TimeStamp = recipe.TimeStamp;
        }
        public JointRecipeTable Recipe => this;

        [Column] public float MVS_Len { get; set; }
        public float MVS_Len_mm => MVS_Len * 1000;
        [Column] public float FinalTorque { get; set; }
        [Column] public float FinalLength { get; set; }
        public float FinalLength_mm => FinalLength * 1000;
        public float FinalMakeupLength_mm => (FinalLength - MVS_Len) * 1000;
        [Column] public float FinalJVal { get; set; }
        [Column] public float FinalTurns { get; set; }
        [Column] public float FinalShoulderTorque { get; set; }
        [Column] public float FinalShoulderTurns { get; set; }
        [Column(DataType = DataType.Long)] public uint ResultPLC { get; set; }
        [Column(DataType = DataType.Long)] public uint ResultTotal { get; set; }
        [Column] public DateTime StartTimeStamp { get; set; }
        [Column] public DateTime FinishTimeStamp { get; set; }

        [Column]
        public byte[] Series
        {
            get             
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    TqTnLenPoint[] pointsArray = PointSeries.ToArray();
                    formatter.Serialize(ms, pointsArray);
                    return ms.ToArray();
                }
            }
            set
            {
                using (MemoryStream ms = new MemoryStream(value))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    PointSeries = new List<TqTnLenPoint>();
                    foreach (var point in (TqTnLenPoint[])formatter.Deserialize(ms))
                    {
                        PointSeries.Add(point);
                    }
                }
            }
        }
        
        public List<TqTnLenPoint> PointSeries { get; set; }

        public EvaluationVerdict EvaluationVerdict { get; set; }


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
            result.StartTimeStamp = StartTimeStamp;
            result.FinishTimeStamp = FinishTimeStamp;
            result.MVS_Len = (float)MVS_Len;

            using (MemoryStream ms = new MemoryStream(Series))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                result.Series = new List<TqTnLenPoint>();
                foreach (var point in (TqTnLenPoint[])formatter.Deserialize(ms))
                {
                    result.Series.Add(point);
                }

                //result.CalculateTurnPerMinute();

            }
           
            return result;  

        }

        public JointResultTable FromJointResult(JointResult result)
        {
            if (result.Recipe != null)
                base.FromJointRecipe(result.Recipe);
            else
                base.FromJointRecipe(new JointRecipe() { Name = "Рецепт не задан"});
            // Переписываем идентификатор, а то будет взят из рецепта
            // это из за того, что было сделано неправильное наследование
            Id = Guid.NewGuid();
            FinalTorque = result.FinalTorque;
            FinalLength = result.FinalLength;
            FinalJVal = result.FinalJVal;
            FinalTurns = result.FinalTurns;
            FinalShoulderTorque = result.FinalShoulderTorque;
            FinalShoulderTurns = result.FinalShoulderTurns;
            ResultPLC = result.ResultPLC;
            ResultTotal = result.ResultTotal;
            StartTimeStamp = result.StartTimeStamp;
            FinishTimeStamp = result.FinishTimeStamp;
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
