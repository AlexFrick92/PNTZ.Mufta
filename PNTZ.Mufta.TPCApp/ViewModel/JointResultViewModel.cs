using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class JointResultViewModel : BaseViewModel
    {
        JointResult ResultModel { get; set; }

        public JointResultViewModel(JointResult result)
        {
            ResultModel = result;
        }

        public string RecipeName { get => ResultModel.Recipe.Name; }

        public string SelectedMode
        {
            get
            {                
                switch (ResultModel.Recipe.JointMode)
                {
                    case JointMode.Torque:
                        return "По моменту";

                    case JointMode.TorqueShoulder:
                        return "По моменту с контролем заплечника";

                    case JointMode.Length:
                        return "По длине";

                    case JointMode.TorqueLength:
                        return "По длине с контролем момента";

                    case JointMode.Jval:
                        return "По значению J";

                    case JointMode.TorqueJVal:
                        return "По значению J с контролем момента";

                    default:
                        return "не выбран";
                }
            }
        }

        public float FinalTorque { get => ResultModel.FinalTorque; }
        public float FinalJVal { get => ResultModel.FinalJVal; }
        public float FinalLength { get => ResultModel.FinalLength; }
        public float FinalTurns { get => ResultModel.FinalTurns; }
        public List<TqTnLenPoint> Series { get => ResultModel.Series; }

        public DateTime FinishTimeStamp { get => ResultModel.FinishTimeStamp; }
    }
}
