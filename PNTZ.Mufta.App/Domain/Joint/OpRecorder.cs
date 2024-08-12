using Promatis.DataPoint.Interface;
using Promatis.DataPoint.Configuration;
using System.Collections.ObjectModel;
using Toolkit.IO;

namespace PNTZ.Mufta.App.Domain.Joint
{
    public class OpRecorder : DpProcessor
    {
        ICliProgram _cli;
        CancellationTokenSource cts;
        public OpRecorder(string name, ICliProgram cli)
        {
            ActualTqTnSeries = new ObservableCollection<TqTnPoint>();
            Name = name;
            cli.RegisterCommand("opStart", (arg) => StartRecording());
            cli.RegisterCommand("opStop", (arg) => cts?.Cancel());
            _cli = cli;
        }
        public IDpValue<TqTnPoint> TqTnPoint { get; set; }
        public event EventHandler NewRecordStarted;
        async void StartRecording()
        {
            //TqTnPoint.ValueUpdated += (s, v) => _cli.WriteLine(Name + ":" + v.ToString());

            cts = new CancellationTokenSource();
            ActualTqTnSeries.Clear();

            NewRecordStarted?.Invoke(this, EventArgs.Empty);

            int sampleNum = 0;
            try
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (cts.IsCancellationRequested)
                        {
                            cts.Token.ThrowIfCancellationRequested();
                        }
                        else
                        {
                            ActualTqTnSeries.Add(new TqTnPoint() { Tq = TqTnPoint.Value.Tq, Tn = TqTnPoint.Value.Tn, TimeStamp = sampleNum * 10 });
                            //ActualTqTnSeries.Add(new TqTnPoint() { Tq = 10, Tn = 10, TimeStamp = sampleNum * 10 });
                            sampleNum++;
                            Thread.Sleep(10);
                        }
                    }
                });
            }
            catch (OperationCanceledException ex)
            {
                _cli.WriteLine("Считывание параметров остановлено");
            }
            catch 
            {
                throw;
            }

        }

        

        public ObservableCollection<TqTnPoint> ActualTqTnSeries { get; set; }



    }
}
