
using Desktop.MVVM;
using DevExpress.Charts.Designer.Native;
using PNTZ.Mufta.App.Domain.Joint;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using static PNTZ.Mufta.App.App;

namespace PNTZ.Mufta.App.ViewModel
{
    public class JointViewModel : BaseViewModel
    {
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

                TqTnSeries = new ObservableCollection<TqTnPoint>();
                OnPropertyChanged(nameof(TqTnSeries));

                AppInstance.ResultObserver.RecordingOperationParamFinished += (s1, v1) => ctc.Cancel();

                await RecordingParams(ctc.Token);
            };



            Task refreshOperationValues = RefreshOperationValues();
        }


        async Task RecordingParams(CancellationToken token)
        {
            const int RECORDING_INTERVAL = 50;
            await Task.Run(() =>
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
                                TimeStamp = stamp 
                            });                            
                        });
                    }

                    Task.Delay(TimeSpan.FromMilliseconds(RECORDING_INTERVAL)).Wait();
                    stamp += RECORDING_INTERVAL;
                }
            });
        }

        async Task RefreshOperationValues()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    ActualTqTnLen = AppInstance.ActualTqTnLen;                    
                    OnPropertyChanged(nameof(ActualTqTnLen));                    
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }
            });
        }

        public JointRecipe JointRecipe { get; set; }

        public JointResult JointResult { get; set; }

        public TqTnLen ActualTqTnLen { get; set; }

        public ObservableCollection<TqTnPoint> TqTnSeries { get; set; }
    }
}
