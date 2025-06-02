using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Desktop.MVVM;

namespace PNTZ.Mufta.TPCApp.View
{
    public class ChartViewConfig : BaseViewModel
    {

        double xMaxValue = 1;
        public double XMaxValue 
        { 
            get => xMaxValue; 
            set
            {
                xMaxValue = value;
                OnPropertyChanged(nameof(XMaxValue));
                XGridSpacing = CalculateGridSpacing(XMinValue, XMaxValue);
            }
        }

        double xMinValue;
        public double XMinValue 
        {
            get => xMinValue;
            set
            {
                xMinValue = value;
                OnPropertyChanged(nameof(XMinValue));
                XGridSpacing = CalculateGridSpacing(XMinValue, XMaxValue);

            }
        }

        public double XGridSpacing { get; protected set; }

        double yMaxValue = 1;
        public double YMaxValue 
        {
            get => yMaxValue;
            set
            {

                yMaxValue = value;
                OnPropertyChanged(nameof(YMaxValue));
                YGridSpacing = CalculateGridSpacing(yMinValue, yMaxValue); 
            }
        }


        double yMinValue;
        public double YMinValue 
        {
            get => yMinValue;
            set
            {
                yMinValue = value;
                OnPropertyChanged(nameof(YMinValue));
                YGridSpacing = CalculateGridSpacing(YMinValue, yMaxValue);
            }
        }

        public double YGridSpacing { get; protected set; }    


        double CalculateGridSpacing(double min, double max)
        {
            return Math.Abs( (max - min) / 10 );
        }


    }
}
