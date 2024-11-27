
using DpConnect;
using PNTZ.Mufta.TPCApp.DpConnect.Struct;
using Promatis.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.DpConnect
{
    public class JointOperationalParam : IDpWorker
    {


        public IDpValue<OperationalParam> DpParam { get; set; }

        ILogger logger;

        public JointOperationalParam(ILogger logger)
        {
            this.logger = logger;
        }

        public void DpBound()
        {
        }

    }
}
