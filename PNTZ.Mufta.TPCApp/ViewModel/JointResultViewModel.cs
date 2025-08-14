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
        public JointResult ResultModel { get; set; }

        public JointResultViewModel(JointResult result)
        {
            ResultModel = result;
        }

        public string RecipeName => ResultModel.Recipe.Name;

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
        public float MVS_Len => ResultModel.MVS_Len * 1000;
        public float FinalMakeUpLength => FinalLength - MVS_Len;
        public float FinalTorque => ResultModel.FinalTorque;
        public float FinalJVal => ResultModel.FinalJVal;
        public float FinalLength => ResultModel.FinalLength * 1000;
        public float FinalTurns => ResultModel.FinalTurns;
        public List<TqTnLenPointViewModel> Series { get => ResultModel.Series.Select(x => new TqTnLenPointViewModel(x)).ToList(); }

        public DateTime FinishTimeStamp => ResultModel.FinishTimeStamp;

        public string Result
        {
            get
            {
                switch (ResultModel.ResultTotal)
                {
                    case 2:
                        return "Брак";
                    case 1:
                        return "Годное";
                    default:
                        return "Не установлен";

                }
            }
        }

        public JointRecipeViewModel Recipe => new JointRecipeViewModel(ResultModel.Recipe);
    }
}
