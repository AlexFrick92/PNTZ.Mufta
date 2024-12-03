using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.DpConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class StatusBarViewModel : BaseViewModel
    {

        public StatusBarViewModel(JointResultDpWorker resultDpWorker, HeartbeatCheck hbWorker)
        {
            ResultDpWorker = resultDpWorker;
            HbCheckWorker = hbWorker;
        }

        HeartbeatCheck hbCheckWorker;
        HeartbeatCheck HbCheckWorker
        {
            get => hbCheckWorker;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                hbCheckWorker = value;

                HbCheckWorker.PlcHeartbeat += (s, v) =>
                {
                    PlcHeartbeat = v;
                    OnPropertyChanged(nameof(PlcHeartbeat));                    

                };

                HbCheckWorker.HeartBeatDisapper += (o, e) =>
                {
                    PlcHeartbeat = false;
                    OnPropertyChanged(nameof(PlcHeartbeat));
                };
                HbCheckWorker.PlcStatusChanged+= (o, e) => 
                {
                    PlcConnected = e;
                    OnPropertyChanged(nameof(PlcConnected));

                };
            }            
        }

        public bool PlcConnected { get; set; }
        public bool PlcHeartbeat { get; set; }

        JointResultDpWorker resultDpWorker;
        JointResultDpWorker ResultDpWorker
        {
            get => resultDpWorker;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                resultDpWorker = value;

                resultDpWorker.PipeAppear += ResultDpWorker_JointBegun;
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
