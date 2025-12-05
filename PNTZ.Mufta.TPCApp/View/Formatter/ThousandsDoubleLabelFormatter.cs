using DevExpress.Xpf.Charts;
using System;

namespace PNTZ.Mufta.TPCApp.View.Formatter
{
    public class ThousandsDoubleLabelFormatter : IAxisLabelFormatter
    {
        public string GetAxisLabelText(object axisValue)
        {
            if (axisValue is double value)
            {
                if (value >= 1000)
                {
                    double shortValue = value / 1000.0;
                    return (shortValue % 1 == 0 ? ((int)shortValue).ToString() : shortValue.ToString("0.#")) + "k";
                }
                else
                {
                    return value.ToString("0.#");
                }

            }
            return axisValue?.ToString(); throw new NotImplementedException();
        }
    }
}
