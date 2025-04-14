using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.View
{
    public class ChartViewConfig
    {

        double xMaxValue = 1;
        public double XMaxValue 
        { 
            get => xMaxValue; 
            set
            {
                xMaxValue = value;
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
