using PNTZ.Mufta.App.Domain.Joint;
using Promatis.DataPoint.Interface;
using Promatis.Desktop.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PNTZ.Mufta.Launcher.ViewModel.Chart
{
    public class ChartViewModel : BaseViewModel, IDpProcessor
    {
        public ObservableCollection<TqTnPoint> Data { get; private set; }
        public string Name { get; set; } = "TqTnChart";

        public ChartViewModel()
        {
            Data = new ObservableCollection<TqTnPoint>();
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
            TqTnPoint.ValueUpdated += (s, v) => AddPoint(v);
        }
        public void AddPoint(TqTnPoint point)
        {
            
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Data.Add(point);
                Console.WriteLine($"Добавлена точка {point.Tn} : {point.Tq} ");
            });
            
        }
    }
}
