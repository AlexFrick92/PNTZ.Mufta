using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Toolbox.Smoothing
{
    internal class MovingAverage
    {
        double[] values;

        public MovingAverage(int window)
        {
            if (window < 2) throw new ArgumentException(nameof(window));

            values = new double[window];
        }

        public double SmoothValue(double newValue)
        {            
            double sum = 0;
            for (int i = 0; i < values.Length - 1; i++)
            {
                values[i] = values[i + 1];
                sum += values[i];
            }
            values[values.Length - 1] = newValue;
            sum += newValue;

            double average = sum / values.Length;                
                
            return average;
            
        }

    }
}
