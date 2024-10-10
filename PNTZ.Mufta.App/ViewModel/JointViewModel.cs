
using Desktop.MVVM;
using DevExpress.Charts.Designer.Native;
using PNTZ.Mufta.App.Domain.Joint;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using static PNTZ.Mufta.App.App;

namespace PNTZ.Mufta.App.ViewModel
{
    public class JointViewModel : BaseViewModel
    {
        CancellationTokenSource ctcStopRefreshing;

        public JointViewModel() 
        {
            if(AppInstance.LoadedRecipe != null)
                JointRecipe = AppInstance.LoadedRecipe;
            else
                JointRecipe = new JointRecipe();    

            if(AppInstance.LastJointResult != null)
                JointResult = AppInstance.LastJointResult;
            else
                JointResult = new JointResult();

            AppInstance.PropertyChanged += (s, rec) =>
            {
                if(rec.PropertyName == nameof(App.LoadedRecipe))
                {
                    JointRecipe = AppInstance.LoadedRecipe;
                    OnPropertyChanged(nameof(JointRecipe));
                }
                if(rec.PropertyName == nameof(App.LastJointResult))
                {
                    JointResult = AppInstance.LastJointResult;
                    OnPropertyChanged(nameof(JointResult));
                }
            };

            AppInstance.ResultObserver.RecordingOperationParamBegun += async (s, v) =>
            {
                CancellationTokenSource ctc = new CancellationTokenSource();

                Console.WriteLine("Записываем график");

                ctcStopRefreshing.Cancel();

                TqTnSeries = new ObservableCollection<TqTnPoint>();
                OnPropertyChanged(nameof(TqTnSeries));

                AppInstance.ResultObserver.RecordingOperationParamFinished += (s1, v1) => ctc.Cancel();

                await RecordingParams(ctc.Token);
                ctcStopRefreshing = new CancellationTokenSource();
                Task continueRefresh = RefreshOperationValues(ctcStopRefreshing.Token);
            };

            AppInstance.ResultObserver.AwaitForEvaluation += (s, v) =>
            {
                EvaluationRequsted = true;
                OnPropertyChanged(nameof(EvaluationRequsted));
            };

            EvaluateGoodCmd = new RelayCommand((p) => AppInstance.ResultObserver.Evaluate(1));

            EvaluateBadCmd = new RelayCommand((p) => AppInstance.ResultObserver.Evaluate(2));


            ctcStopRefreshing = new CancellationTokenSource();
            
            Task refreshOperationValues = RefreshOperationValues(ctcStopRefreshing.Token);
            

        }



        async Task RecordingParams(CancellationToken token)
        {
            DateTime beginTime = DateTime.Now;

            const int RECORDING_INTERVAL = 50;
            await Task.Run(async () =>
            {
                int stamp = 0;
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Запись графика завершена");                        
                        break;
                    }
                    else
                    {                        
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            TqTnSeries.Add(new TqTnPoint() 
                            { 
                                Torque = AppInstance.ActualTqTnLen.Torque, 
                                Turns = AppInstance.ActualTqTnLen.Turns,
                                Length = AppInstance.ActualTqTnLen.Length,                                
                                TimeStamp = Convert.ToInt32(DateTime.Now.Subtract(beginTime).TotalMilliseconds)
                            });
                            ActualTqTnLen = AppInstance.ActualTqTnLen;
                            OnPropertyChanged(nameof(ActualTqTnLen));
                        });
                    }
                    await Task.Delay(TimeSpan.FromMilliseconds(RECORDING_INTERVAL));                    
                }
            }, token);
        }

        async Task RefreshOperationValues(CancellationToken token)
        {
            await Task.Run(async () =>
            {
                Console.WriteLine("Обновление данных начато");
                while (true)
                {
                    if(token.IsCancellationRequested)
                    {
                        Console.WriteLine("Обновление данных остановлено");
                        break;
                    }
                    else
                    {
                        ActualTqTnLen = AppInstance.ActualTqTnLen;                    
                        OnPropertyChanged(nameof(ActualTqTnLen));
                        await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
                    }
                }
            }, token);
        }

        public JointRecipe JointRecipe { get; set; }

        public JointResult JointResult { get; set; }

        public TqTnLen ActualTqTnLen { get; set; }

        public ObservableCollection<TqTnPoint> TqTnSeries { get; set; }

        public ICommand EvaluateGoodCmd { get; private set; }
        public ICommand EvaluateBadCmd { get; private set; }
        public bool EvaluationRequsted { get; set; } = false;
    }
}
