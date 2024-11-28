using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.DpConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class StatusBarViewModel : BaseViewModel
    {

        public StatusBarViewModel(JointResultDpWorker resultDpWorker)
        {
            ResultDpWorker = resultDpWorker;
        }


        JointResultDpWorker resultDpWorker;
        JointResultDpWorker ResultDpWorker
        {
            get => resultDpWorker;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                resultDpWorker = value;

                resultDpWorker.JointBegun += ResultDpWorker_JointBegun;
                resultDpWorker.JointFinished += ResultDpWorker_JointFinished;
            }
               
        }

        public bool JointInProgress { get; set; } = false;
        private void ResultDpWorker_JointFinished(object sender, EventArgs e)
        {
            JointInProgress = false;
            OnPropertyChanged(nameof(JointInProgress));
        }

        private void ResultDpWorker_JointBegun(object sender, EventArgs e)
        {
            JointInProgress = true;
            OnPropertyChanged(nameof(JointInProgress));
        }

    }
}
