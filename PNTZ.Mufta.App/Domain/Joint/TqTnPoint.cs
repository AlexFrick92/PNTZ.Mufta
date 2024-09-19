namespace PNTZ.Mufta.App.Domain.Joint
{
    public class TqTnPoint
    {
        public float Torque { get; set; }
        public float Turns { get; set; }
        public float Len { get; set; }
        public int TimeStamp { get; set; }

        public override string ToString()
        {
            return Torque.ToString() + " : " + Turns.ToString();
        }
    }
}
