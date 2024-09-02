using DpConnect.Interface;
using Toolkit.IO;

using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;


namespace PNTZ.Mufta.App.Domain.Joint
{
    public class OpRecorder : DpProcessor
    {
        ICliProgram _cli;
        CancellationTokenSource cts;
        Task recordingTask;
        public OpRecorder(string name, ICliProgram cli)
        {
            ActualTqTnSeries = new ObservableCollection<TqTnPoint>();
            Name = name;
            cli.RegisterCommand("opStart", (arg) => StartRecording());
            cli.RegisterCommand("opStop", (arg) => StopRecording());
            _cli = cli;
        }

        public IDpValue<TqTnPoint> TqTnPoint { get; set; }    
        
        public event EventHandler NewRecordStarted;
        public event EventHandler<ObservableCollection<TqTnPoint>> RecordingDone;
        async void StartRecording()
        {

            if(recordingTask != null)
            {
                _cli.WriteLine("Уже идет запись точек!");
                return;
            }

            _cli.WriteLine("Начинаем запись точек..");
            cts = new CancellationTokenSource();
            ActualTqTnSeries.Clear();

            NewRecordStarted?.Invoke(this, EventArgs.Empty);

            int sampleNum = 0;

            recordingTask = Task.Run(() =>
            {
                while (true)
                {
                    cts.Token.ThrowIfCancellationRequested();

                    ActualTqTnSeries.Add(new TqTnPoint() { Tq = TqTnPoint.Value.Tq, Tn = TqTnPoint.Value.Tn, TimeStamp = sampleNum * 10 });                        
                    sampleNum++;
                    Thread.Sleep(10);
                    
                };
            });

            try
            {
                await recordingTask;
            }
            catch (OperationCanceledException ex)
            {
                _cli.WriteLine("Запись точек остановлена.");
            }            
            finally
            {
                RecordingDone(this, ActualTqTnSeries);
                cts?.Dispose();
                recordingTask?.Dispose();
                recordingTask = null;
            }

        }
        async void StopRecording()
        {
            cts?.Cancel();            
        }
        public ObservableCollection<TqTnPoint> ActualTqTnSeries { get; set; }
    }
}
