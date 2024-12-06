using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
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

        public StatusBarViewModel(JointResultDpWorker resultDpWorker, HeartbeatCheck hbWorker, RecipeToPlc recWorker)
        {
            ResultDpWorker = resultDpWorker;
            HbCheckWorker = hbWorker;
            RecWorker = recWorker;
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

        public bool RecipeLoaded { get; set; }
        public JointRecipe LoadedRecipe { get; set; }
        RecipeToPlc recWorker;
        RecipeToPlc RecWorker
        {
            get => recWorker;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                recWorker = value;

                recWorker.RecipeLoaded += (s, e) =>
                {
                    RecipeLoaded = true;
                    OnPropertyChanged(nameof(RecipeLoaded));
                    LoadedRecipe = e;
                    OnPropertyChanged(nameof(LoadedRecipe));
                };
            }
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

                resultDpWorker.PipeAppear += ResultDpWorker_JointBegun;
                resultDpWorker.RecordingBegun += ResultDpWorker_RecordingBegun;
                resultDpWorker.AwaitForEvaluation += ResultDpWorker_AwaitForEvaluation;
                resultDpWorker.RecordingFinished += ResultDpWorker_RecordingFinished;
                resultDpWorker.JointFinished += ResultDpWorker_JointFinished;
            }
               
        }
        public string JointStatus { get; set; } = "";
        public bool JointInProgress { get; set; } = false;
        private void ResultDpWorker_RecordingFinished(object sender, EventArgs e)
        {
            JointStatus = "Запись завершена";
            OnPropertyChanged(nameof(JointStatus));
        }

        private void ResultDpWorker_AwaitForEvaluation(object sender, EventArgs e)
        {
            JointStatus = "Ожидание оценки оператора";
            OnPropertyChanged(nameof(JointStatus));
        }

        private void ResultDpWorker_RecordingBegun(object sender, EventArgs e)
        {
            JointStatus = "Запись параметров начата";
            OnPropertyChanged(nameof(JointStatus));
        }

        private void ResultDpWorker_JointFinished(object sender, EventArgs e)
        {
            JointInProgress = false;
            OnPropertyChanged(nameof(JointInProgress));

            JointStatus = "Свинчивание завершено";
            OnPropertyChanged(nameof(JointStatus));
        }

        private void ResultDpWorker_JointBegun(object sender, EventArgs e)
        {
            JointInProgress = true;
            OnPropertyChanged(nameof(JointInProgress));
            JointStatus = "Труба в позиции";
            OnPropertyChanged(nameof(JointStatus));
        }

    }
}
