using DpConnect;


namespace PNTZ.Mufta.TPCApp.DpConnect
{
    public class MakeHeartBeat : IMakeHeartBeat
    {

        public IDpValue<bool> DpHeartbeat { get; set; }

        public void DpBound()
        {
            
        }
    }
}
