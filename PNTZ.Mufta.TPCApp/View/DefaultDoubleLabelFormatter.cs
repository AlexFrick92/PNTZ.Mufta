using DevExpress.Xpf.Charts;
using System;

namespace PNTZ.Mufta.TPCApp.View
{
    public class DefaultDoubleLabelFormatter : IAxisLabelFormatter
    {
        public string GetAxisLabelText(object axisValue)
        {
            if (axisValue is double value)
            {
                var rounded = Math.Round(value / 1000);

                return value.ToString("N0");                
            }
            return axisValue?.ToString(); throw new NotImplementedException();
        }
    }
}
