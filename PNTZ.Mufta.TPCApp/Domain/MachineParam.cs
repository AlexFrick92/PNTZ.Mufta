using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Domain
{
    public class MachineParam
    {
        public float MP_Load_Cell_Span { get; set; }
        public float MP_Load_Span_Digits { get; set; }
        public float MP_Handle_Length { get; set; }
        public float MP_Handle_Length_Digits { get; set; }
        public float MP_TC_PPR { get; set; }
        public float MP_Box_Length { get; set; }
        public float MP_Box_Length_Digit { get; set; }
        public float MP_Makeup_Length { get; set; }
        public float MP_Makeup_Length_Digits { get; set; }
        public float MP_Tq_Max { get; set; }
        public string MP_Machine_No { get; set; }
        public float MP_Cal_Factor { get; set; }
        public string MP_Cal_User { get; set; }
        public DateTime MP_Cal_Timestamp { get; set; }
        public float MP_Makeup_Length_Offset { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("*Актуальные параметры машин*");
            stringBuilder.AppendLine("MP_Load_Cell_Span: " + MP_Load_Cell_Span);
            stringBuilder.AppendLine("MP_Load_Span_Digits: " + MP_Load_Span_Digits);
            stringBuilder.AppendLine("MP_Handle_Length: " + MP_Handle_Length);
            stringBuilder.AppendLine("MP_Handle_Length_Digits: " + MP_Handle_Length_Digits);
            stringBuilder.AppendLine("MP_TC_PPR: " + MP_TC_PPR);
            stringBuilder.AppendLine("MP_Box_Length: " + MP_Box_Length);
            stringBuilder.AppendLine("MP_Box_Length_Digit: " + MP_Box_Length_Digit);
            stringBuilder.AppendLine("MP_Makeup_Length: " + MP_Makeup_Length);
            stringBuilder.AppendLine("MP_Makeup_Length_Digits: " + MP_Makeup_Length_Digits);
            stringBuilder.AppendLine("MP_Tq_Max: " + MP_Tq_Max);
            stringBuilder.AppendLine("MP_Machine_No: " + MP_Machine_No);
            stringBuilder.AppendLine("MP_Cal_Factor: " + MP_Cal_Factor);
            stringBuilder.AppendLine("MP_Cal_User: " + MP_Cal_User);
            stringBuilder.AppendLine("MP_Cal_Timestamp: " + MP_Cal_Timestamp);
            stringBuilder.AppendLine("MP_Makeup_Length_Offset: " + MP_Makeup_Length_Offset);

            return stringBuilder.ToString();

        }
    }
}
