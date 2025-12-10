using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Domain
{
    /// <summary>
    /// Данные оценки соединения    
    /// </summary>
    public class EvaluationVerdict
    {
        /// <summary>
        /// Момент в допуске
        /// </summary>
        public bool TorqueOk { get; set; }
        /// <summary>
        /// Глубина в допуске
        /// </summary>
        public bool LentghOk { get; set; }
        /// <summary>
        /// Заплечник в допуске
        /// </summary>
        public bool ShoulderOk { get; set; }

    }
}
