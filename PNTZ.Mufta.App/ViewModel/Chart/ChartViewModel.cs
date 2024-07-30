using Promatis.Desktop.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.Launcher.ViewModel.Chart
{
    public class ChartViewModel : BaseViewModel
    {
        public ObservableCollection<DataPoint> Data { get; private set; }

        public ChartViewModel()
        {
            Data = new ObservableCollection<DataPoint>() {
                new DataPoint (new DateTime(2013,12,31), 362.5),
                new DataPoint (new DateTime(2014,12,31), 348.4),
                new DataPoint (new DateTime(2015,12,31), 279.0),
                new DataPoint (new DateTime(2016,12,31), 230.9),
                new DataPoint (new DateTime(2017,12,31), 203.5),
                new DataPoint (new DateTime(2018,12,31), 197.1)
            };
        }
        public class DataSeries
        {
            public string Name { get; set; }
            public ObservableCollection<DataPoint> Values { get; set; }
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
    }
}
