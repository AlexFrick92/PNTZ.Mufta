using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.App.Domain
{
    public class MachineParameters
    {
        public float MP_Load_Cell_Span { get; set; }
        public float MP_Load_Span_Digits { get; set; }
        public float MP_Handle_Length { get; set; }
        public float MP_Handle_Length_Digits { get; set; }
        public float MP_TC_PPR { get; set; }
        public float MP_Box_Length { get; set; }
        public float MP_Box_Length_Digit { get; set; }
        public float MP_Makeup_Length   { get; set; }
        public float MP_Makeup_Length_Digits { get; set; }
        public float MP_Tq_Max { get; set; }
        public string MP_Machine_No { get; set; }
        public float MP_Cal_Factor { get; set; }
        public string MP_Cal_User { get; set; }
        public DateTime MP_Cal_Timestamp { get; set; }
        public float MP_Makeup_Length_Offset { get; set; }
    }
}
