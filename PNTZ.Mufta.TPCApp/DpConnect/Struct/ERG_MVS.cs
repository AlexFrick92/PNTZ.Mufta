using System;

namespace PNTZ.Mufta.TPCApp.DpConnect.Struct
{
    public class ERG_MVS
    {
        public uint PMR_PIPE_POS { get; set; }

        public float PMR_PIPE_POS_LEN { get; set; }

        public DateTime PMR_Pre_MAKEUP_BEGIN { get; set; }

        public DateTime PMR_Pre_MAKEUP_END { get; set; }

        public float PMR_Pre_MAKEUP_LEN { get; set; }

        public uint PMR_Pre_MAKEUP_RESULT { get; set; }

    }
}
