using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.Repository;
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

        public StatusBarViewModel(JointProcessDpWorker resultDpWorker, HeartbeatCheck hbWorker, IRecipeTableLoader recLoader, SensorStatusDpWorker sensorWorker)
        {
            ResultDpWorker = resultDpWorker;
            HbCheckWorker = hbWorker;
            
            RecipeLoader = recLoader;

            SensorWorker = sensorWorker;
        }


        private SensorStatusDpWorker _sensorWorker;

        public SensorStatusDpWorker SensorWorker
        {
            get => _sensorWorker;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                _sensorWorker = value;

                _sensorWorker.DpAED.ValueUpdated += (s, v) => { SensorAedOk = v; OnPropertyChanged(nameof(SensorAedOk)); };
                _sensorWorker.DpRotate.ValueUpdated += (s, v) => { SensorRotateOk = v; OnPropertyChanged(nameof(SensorRotateOk)); };
                _sensorWorker.DpLength.ValueUpdated += (s, v) => { SensorLengthOk = v; OnPropertyChanged(nameof(SensorLengthOk)); };


            }
        }

        public bool SensorAedOk { get; set; }
        public bool SensorRotateOk { get; set; }
        public bool SensorLengthOk { get; set; }


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


        //Загруженный рецепт
        public bool RecipeLoaded { get; set; } //Пока рецепт не загруежен, покажем другой текст.
        //В перспективе, убрать эту переменную и отображать текст основываясь на null?
        public JointRecipeTable LoadedRecipe { get => RecipeLoader.LoadedRecipe; }

        IRecipeTableLoader recipeLoader;
        IRecipeTableLoader RecipeLoader
        {
            get => recipeLoader;
            set
            {
                recipeLoader = value;

                recipeLoader.RecipeLoaded += (s, r) =>
                {
                    RecipeLoaded = true;
                    OnPropertyChanged(nameof(RecipeLoaded));
                    OnPropertyChanged(nameof(LoadedRecipe));
                };
            }
        }
        //---

        JointProcessDpWorker resultDpWorker;
        JointProcessDpWorker ResultDpWorker
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
        private void ResultDpWorker_RecordingFinished(object sender, JointResult e)
        {
            JointStatus = "Запись завершена";
            OnPropertyChanged(nameof(JointStatus));
        }

        private void ResultDpWorker_AwaitForEvaluation(object sender, JointResult e)
        {
            JointStatus = "Ожидание оценки оператора";
            OnPropertyChanged(nameof(JointStatus));
        }

        private void ResultDpWorker_RecordingBegun(object sender, EventArgs e)
        {
            JointStatus = "Запись параметров начата";
            OnPropertyChanged(nameof(JointStatus));
        }

        private void ResultDpWorker_JointFinished(object sender, JointResult e)
        {
            JointInProgress = false;
            OnPropertyChanged(nameof(JointInProgress));

            JointStatus = "Свинчивание завершено";
            OnPropertyChanged(nameof(JointStatus));
        }

        private void ResultDpWorker_JointBegun(object sender, JointResult e)
        {
            JointInProgress = true;
            OnPropertyChanged(nameof(JointInProgress));
            JointStatus = "Труба в позиции";
            OnPropertyChanged(nameof(JointStatus));
        }

    }
}
