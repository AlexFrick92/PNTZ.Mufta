using DevExpress.Xpf.Charts;
using System;

namespace PNTZ.Mufta.TPCApp.View.Formatter
{
    public class SecondsIntegerLabelFormatter : IAxisLabelFormatter
    {
        public string GetAxisLabelText(object axisValue)
        {
            double milliseconds = 0;

            if (axisValue is int intValue)
            {
                milliseconds = intValue;
            }
            else if (axisValue is double doubleValue)
            {
                milliseconds = doubleValue;
            }
            else
            {
                return axisValue?.ToString() ?? string.Empty;
            }

            double seconds = milliseconds / 1000.0;
            return (seconds % 1 == 0 ? ((int)seconds).ToString() : seconds.ToString("0.#")) + " сек";
        }
    }
}
