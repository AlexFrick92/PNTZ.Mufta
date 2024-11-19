using System;


namespace PNTZ.Mufta.TPCApp.DpConnect.Struct
{
    public class ERG_CAM
    {
        public DateTime PMR_MR_MAKEUP_BEGIN  { get; set; }

        public DateTime PMR_MR_MAKEUP_END { get; set; }

        public float PMR_MR_MAKEUP_FIN_TN { get; set; }

        public float PMR_MR_MAKEUP_FIN_TQ { get; set; }

        public float PMR_MR_MAKEUP_LEN { get; set; }

        public uint PMR_MR_MAKEUP_RESULT { get; set; }

        public float PMR_MR_TOTAL_MAKEUP_LEN { get; set; }

        public float PMR_MR_TOTAL_MAKEUP_VAL { get; set; }

        public uint PMR_MR_TOTAL_RESULT { get; set; }
    }
}
