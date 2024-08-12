using Azure.Identity;
using PNTZ.Mufta.App.Domain.Joint;
using Promatis.DataPoint.Interface;
using Promatis.Desktop.MVVM;
using System.Collections.ObjectModel;


namespace PNTZ.Mufta.Launcher.ViewModel.Chart
{
    public class ChartViewModel : BaseViewModel, IDpProcessor
    {
        public ObservableCollection<TqTnPoint> Data { get; private set; }
        CancellationTokenSource cts;
        public string Name { get; set; } = "TqTnChart";

        OpRecorder OpRecorder;
        public ChartViewModel(OpRecorder opRecorder)
        {
            OpRecorder = opRecorder;
        }       
        public class DataPoint
        {
            public DateTime Argument { get; set; }
            public double Value { get; set; }
            public DataPoint(DateTime argument, double value)
            {
                Argument = argument;
                Value = value;
            }
        }

        public IDpValue<TqTnPoint> TqTnPoint { get; set; }

        public void OnDpInitialized()
        {            
            cts = new CancellationTokenSource();
            OpRecorder.NewRecordStarted += DecNewPointsAsync;
        }

        private void NewRecordStarted(object o, EventArgs e)
        {
            Console.WriteLine("Подписались!");
            Data = new ObservableCollection<TqTnPoint>();
            OnPropertyChanged(nameof(Data));
            OpRecorder.ActualTqTnSeries.CollectionChanged += (s, a) =>
            {
                if (a.NewItems?.Count >= 1)
                    foreach (TqTnPoint point in a.NewItems)
                    {

                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            Data.Add(point);
                            Console.WriteLine($"Добавлена точка {point.Tn} : {point.Tq} : {point.TimeStamp}");
                        });
                    }
            };
        }
        int tnum = 0;
        Task currentTask = Task.CompletedTask;    
        private async void DecNewPointsAsync(object o, EventArgs e)
        {
            tnum++;
            if (!currentTask.IsCompleted)
            {
                cts.Cancel();
                try
                {
                    await currentTask;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Предыдущая задача была отменена");
                }
                finally
                {
                    cts.Dispose();
                    cts = new CancellationTokenSource();
                }
            }
            try
            {
                currentTask =  Task.Run(() =>
                {
                    AddingPoint(cts.Token);
                }, cts.Token);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("Запись точек прервана");
                cts.Dispose();
            }
        }
        private void AddingPoint(CancellationToken token)
        {
            int tnumt = tnum;
            Data = new ObservableCollection<TqTnPoint>();
            OnPropertyChanged(nameof(Data));
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                Console.WriteLine($"Считываем точку {tnumt}");
                while (OpRecorder.DataQueue.TryDequeue(out TqTnPoint point))
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Data.Add(point);
                        Console.WriteLine($"{tnumt} Добавлена точка {point.Tn} : {point.Tq} : {point.TimeStamp}");
                    });
                    Console.WriteLine("Считали точку");
                }

                Thread.Sleep(1000);
            }

        }

    }
}
