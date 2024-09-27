using DevExpress.Xpf.Utils.Themes;
using DpConnect.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.App.Domain
{
    public class CommonParamObserver : DpProcessor
    {
        public CommonParamObserver()
        {
            DpInitialized += CommonParamObserver_DpInitialized;
        }

        private void CommonParamObserver_DpInitialized(object sender, EventArgs e)
        {
        }

        public IDpValue<uint> CAM_LOG_NO { get; set; }
        public IDpValue<uint> CAM_CON_NO { get; set; }

        public IDpValue<uint> CAM_OPERATE_MODE { get; set; }

        public IDpValue<uint> CAM_TPC_READY { get; set; }

    }
}
