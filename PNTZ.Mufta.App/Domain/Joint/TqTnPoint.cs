namespace PNTZ.Mufta.App.Domain.Joint
{
    public class TqTnPoint
    {
        public float Torque { get; set; }
        public float Turns { get; set; }
        public float Length { get; set; }
        public int TimeStamp { get; set; }

        public override string ToString()
        {
            return Torque.ToString() + " : " + Turns.ToString() + " : " + Length.ToString() + " : " + TimeStamp.ToString();
        }
    }
}
