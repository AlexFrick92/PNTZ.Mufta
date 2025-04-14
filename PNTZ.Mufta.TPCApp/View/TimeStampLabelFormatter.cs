using DevExpress.Xpf.Charts;
using System;


namespace PNTZ.Mufta.TPCApp.View
{
    internal class TimeStampLabelFormatter : IAxisLabelFormatter
    {
        public string GetAxisLabelText(object axisValue)
        {
            if (axisValue is double value)
            {

                //Метки времени в мс. Мы покажем время в секундах
                var valuesec = value / 1000;

                return valuesec.ToString() + "сек.";                

            }
            return axisValue.ToString();
        }
    }
}
