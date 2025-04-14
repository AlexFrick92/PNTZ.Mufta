using DevExpress.Xpf.Charts;
using System;


namespace PNTZ.Mufta.TPCApp.View
{
    internal class LengthLabelFormatter : IAxisLabelFormatter
    {
        public string GetAxisLabelText(object axisValue)
        {
            if (axisValue is double value)
            {
                var rounded = Math.Round(value / 1000);

                return value.ToString("N0");
                return $"{rounded} к";

            }
            return axisValue?.ToString(); throw new NotImplementedException();
        }
    }
}
