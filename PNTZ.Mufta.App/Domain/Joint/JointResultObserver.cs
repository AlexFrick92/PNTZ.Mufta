using DpConnect.Interface;

using static PNTZ.Mufta.App.App;

namespace PNTZ.Mufta.App.Domain.Joint
{
    public class JointResultObserver : DpProcessor
    {
        public JointResultObserver()
        {
            DpInitialized += (s, e) =>
            {
                ObservingJointResult.ValueUpdated += (s1, v) =>
                {
                    AppInstance.LastJointResult = v;   
                };
            };
        }

        public IDpValue<JointResult> ObservingJointResult {  get; set; }
    }
}
