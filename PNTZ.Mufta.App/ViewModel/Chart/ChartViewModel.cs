using PNTZ.Mufta.App.Domain.Joint;
using Promatis.DataPoint.Interface;
using Promatis.Desktop.MVVM;
using System.Collections.ObjectModel;


namespace PNTZ.Mufta.Launcher.ViewModel.Chart
{
    public class ChartViewModel : BaseViewModel, IDpProcessor
    {
        public ObservableCollection<TqTnPoint> Data { get; private set; }
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
            OpRecorder.NewRecordStarted += NewRecordStarted;
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

    }
}
