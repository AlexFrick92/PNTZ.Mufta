using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Domain
{
    /// <summary>
    /// Детектор точки контакта муфты с заплечником (буртиком) трубы.
    /// Анализирует график зависимости момента от длины/оборотов для определения точки резкого возрастания момента.
    /// </summary>
    public class ShoulderPointDetector
    {
        private List<TqTnLenPoint> _series;

        public ShoulderPointDetector(List<TqTnLenPoint> series)
        {
            _series = series ?? throw new ArgumentNullException(nameof(series));
        }


    }
}
