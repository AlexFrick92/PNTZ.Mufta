using System;

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

using Desktop.MVVM;

using DpConnect.Interface;

using PNTZ.Mufta.App.Domain.Joint;

namespace PNTZ.Mufta.App.ViewModel.Chart
{
    public class ChartViewModel : BaseViewModel, IDpProcessor
    {
        public ObservableCollection<TqTnPoint> Data { get; private set; }
        public ConcurrentQueue<TqTnPoint> DataQueue = new ConcurrentQueue<TqTnPoint>();
        CancellationTokenSource ctsDequeuePoint = new CancellationTokenSource();
        public string Name { get; set; } = "TqTnChart";

        OpRecorder OpRecorder;
        Task DequeuePoint = Task.CompletedTask;
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
            OpRecorder.NewRecordStarted += (o, e) =>
            {
                OpRecorder.ActualTqTnSeries.CollectionChanged += EnqueuPoints;
                BeginDequeuePoint();
            };
            OpRecorder.RecordingDone += (o, tqtn) =>
            {
                StopDequeuePoint();
                OpRecorder.ActualTqTnSeries.CollectionChanged -= EnqueuPoints;
            };

        }

        private void EnqueuPoints(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TqTnPoint point in e.NewItems)
                    DataQueue.Enqueue(point);
        }

        private async void BeginDequeuePoint()
        {
            if (!DequeuePoint.IsCompleted)
                StopDequeuePoint().Wait();

            DequeuePoint = Task.Run(() => AddPoint(ctsDequeuePoint.Token)
            , ctsDequeuePoint.Token);

            try
            {
                await DequeuePoint;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Dequeu is canceled");
            }
            finally
            {
                ctsDequeuePoint.Dispose();
                ctsDequeuePoint = new CancellationTokenSource();
            }
        }
        private void AddPoint(CancellationToken token)
        {
            Data = new ObservableCollection<TqTnPoint>();
            OnPropertyChanged(nameof(Data));
            while (true)
            {
                token.ThrowIfCancellationRequested();
                while (DataQueue.TryDequeue(out TqTnPoint point))
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Data.Add(point);
                    });
                }

                Thread.Sleep(10);
            }

        }
        private async Task StopDequeuePoint()
        {

            ctsDequeuePoint?.Cancel();
        }
    }
}
