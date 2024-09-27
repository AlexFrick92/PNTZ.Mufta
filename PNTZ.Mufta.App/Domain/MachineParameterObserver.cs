using DpConnect.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.App.Domain
{
    public class MachineParameterObserver : DpProcessor
    {

        public MachineParameterObserver()
        {
            DpInitialized += MachineParameterObserver_DpInitialized;
                
                
        }

        private void MachineParameterObserver_DpInitialized(object sender, EventArgs e)
        {
            ObservableMachineParameters.ValueUpdated += ObservableMachineParameters_ValueUpdated;
        }

        private void ObservableMachineParameters_ValueUpdated(object sender, MachineParameters e)
        {
            Console.WriteLine("Обновлены параметры машин: "
                + e.MP_Load_Cell_Span + "\n\r"                
                + e.MP_Load_Span_Digits + "\n\r"
                + e.MP_Handle_Length + "\n\r"
                + e.MP_Handle_Length_Digits + "\n\r"
                + e.MP_TC_PPR + "\n\r"
                + e.MP_Box_Length + "\n\r"
                + e.MP_Box_Length_Digit + "\n\r"
                + e.MP_Makeup_Length + "\n\r"
                + e.MP_Makeup_Length_Digits + "\n\r"
                + e.MP_Tq_Max + "\n\r"
                + e.MP_Machine_No + "\n\r"
                + e.MP_Cal_Factor + "\n\r"
                + e.MP_Cal_User + "\n\r"
                + e.MP_Cal_Timestamp + "\n\r"
                + e.MP_Makeup_Length_Offset + "\n\r"
                );
        }

        public IDpValue<MachineParameters> ObservableMachineParameters { get; set; }
    }
}
